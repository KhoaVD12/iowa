using Azure;
using Iowa.Databases.App;
using Iowa.Databases.App.Tables.Package;
using Iowa.Models.PaginationResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Wolverine;

namespace Iowa.Packages;

[ApiController]
[Route("api/packages")]
public class Controller : ControllerBase
{
    private readonly IMessageBus _messageBus;
    private readonly ILogger<Controller> _logger;
    private readonly IowaContext _context;
    private readonly IHubContext<Hub> _hubContext;

    public Controller(IMessageBus messageBus, ILogger<Controller> logger, IowaContext context, IHubContext<Hub> hubContext)
    {
        _messageBus = messageBus;
        _logger = logger;
        _context = context;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Get.Parameters parameters)
    {

        var query = _context.Packages.AsQueryable();
        var all = query;

        if (parameters.Id.HasValue)
            query = query.Where(x => x.Id == parameters.Id);

        if (parameters.ProviderId.HasValue)
            query = query.Where(x => x.ProviderId == parameters.ProviderId);

        if (!string.IsNullOrEmpty(parameters.Name))
            query = query.Where(x => x.Name.Contains(parameters.Name));

        if (!string.IsNullOrEmpty(parameters.Description))
            query = query.Where(x => x.Description.Contains(parameters.Description));

        if (!string.IsNullOrEmpty(parameters.IconUrl))
            query = query.Where(x => x.IconUrl.Contains(parameters.IconUrl));

        if (parameters.Price.HasValue)
            query = query.Where(x => x.Price == parameters.Price);

        if (!string.IsNullOrEmpty(parameters.Currency))
            query = query.Where(x => x.Currency.Contains(parameters.Currency));

        if (parameters.CreatedDate.HasValue)
            query = query.Where(x => x.CreatedDate == parameters.CreatedDate);

        if (parameters.LastUpdated.HasValue)
            query = query.Where(x => x.LastUpdated == parameters.LastUpdated);

        if (parameters.CreatedById.HasValue)
            query = query.Where(x => x.CreatedById == parameters.CreatedById);

        if (parameters.UpdatedById.HasValue)
            query = query.Where(x => x.UpdatedById == parameters.UpdatedById);

        //if (!string.IsNullOrEmpty(parameters.SortBy))
        //{
        //    var sortBy = typeof(Databases.App.Tables.Package.Table)
        //        .GetProperties()
        //        .FirstOrDefault(p => p.Name.Equals(parameters.SortBy, StringComparison.OrdinalIgnoreCase))
        //        ?.Name;
        //    if (sortBy != null)
        //    {
        //        query = parameters.SortOrder?.ToLower() == "desc"
        //            ? query.OrderByDescending(x => EF.Property<object>(x, sortBy))
        //            : query.OrderBy(x => EF.Property<object>(x, sortBy));
        //    }
        //}
        if (parameters.PageSize.HasValue && parameters.PageIndex.HasValue && parameters.PageSize > 0 && parameters.PageIndex.Value >= 0)
            query = query.Skip(parameters.PageSize.Value * parameters.PageIndex.Value).Take(parameters.PageSize.Value);

        var packages = await query.AsNoTracking().ToListAsync();

          var paginationResults = new Builder<Table>()
           .WithAll(await all.CountAsync())
           .WithIndex(parameters.PageIndex)
           .WithSize(parameters.PageSize)
           .WithTotal(packages.Count)
           .WithItems(packages)
           .Build();

        //không có bắn message
        return Ok(paginationResults);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Post.Payload payload)
    {
        var existingProvider = await _context.Providers.FindAsync(payload.ProviderId);
        if (existingProvider == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Provider not found",
                Detail = $"Provider with ID {payload.ProviderId} does not exist.",
                Status = StatusCodes.Status404NotFound,
                Instance = HttpContext.Request.Path
            });
        }
        var package = new Table
        {
            Id = Guid.NewGuid(),
            ProviderId = payload.ProviderId,
            Name = payload.Name,
            Description = payload.Description,
            IconUrl = payload.IconUrl,
            Price = payload.Price,
            Currency = payload.Currency,
            CreatedById = Guid.NewGuid(),
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        _context.Packages.Add(package);
        await _context.SaveChangesAsync();
        await _messageBus.PublishAsync(new Post.Messager.Message(package.Id));
        await _hubContext.Clients.All.SendAsync("package-created", package.Id);
        return CreatedAtAction(nameof(Get), package.Id);
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] Put.Payload payload)
    {
        var package = await _context.Packages.FindAsync(payload.Id);
        if (package == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Pakage not found",
                Detail = $"Pakage with ID {payload.Id} does not exist.",
                Status = StatusCodes.Status404NotFound,
                Instance = HttpContext.Request.Path
            });
        }

        var existingProvider = await _context.Providers.FindAsync(payload.ProviderId);
        if (existingProvider == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Provider not found",
                Detail = $"Provider with ID {payload.ProviderId} does not exist.",
                Status = StatusCodes.Status404NotFound,
                Instance = HttpContext.Request.Path
            });
        }

        package.ProviderId = payload.ProviderId;
        package.Name = payload.Name;
        package.Description = payload.Description;
        package.IconUrl = payload.IconUrl;
        package.Price = payload.Price;
        package.Currency = payload.Currency;
        package.UpdatedById = Guid.NewGuid();
        package.LastUpdated = DateTime.UtcNow;
        _context.Packages.Update(package);
        await _context.SaveChangesAsync();
        await _messageBus.PublishAsync(new Put.Messager.Message(payload.Id));
        await _hubContext.Clients.All.SendAsync("package-updated", payload.Id);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] Delete.Parameters parameters)
    {
        var package = await _context.Packages.FindAsync(parameters.Id);
        if (package == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Package not found",
                Detail = $"Package with ID {parameters.Id} does not exist.",
                Status = StatusCodes.Status404NotFound,
                Instance = HttpContext.Request.Path
            });
        }

        _context.Packages.Remove(package);
        await _context.SaveChangesAsync();
        await _messageBus.PublishAsync(new Delete.Messager.Message(parameters.Id));
        await _hubContext.Clients.All.SendAsync("package-deleted", parameters.Id);
        return NoContent();
    }
}
