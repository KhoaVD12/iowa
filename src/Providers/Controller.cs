using Iowa.Databases.App;
using Iowa.Databases.App.Tables.Provider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace Iowa.Providers;

[ApiController]
[Route("api/providers")]
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
        var query = _context.Providers.AsQueryable();

        if (parameters.Id.HasValue)
            query = query.Where(x => x.Id == parameters.Id.Value);

        if (!string.IsNullOrEmpty(parameters.Name))
            query = query.Where(x => x.Name.Contains(parameters.Name));

        if (!string.IsNullOrEmpty(parameters.Description))
            query = query.Where(x => x.Description.Contains(parameters.Description));

        if (!string.IsNullOrEmpty(parameters.IconUrl))
            query = query.Where(x => x.IconUrl.Contains(parameters.IconUrl));

        if (!string.IsNullOrEmpty(parameters.WebsiteUrl))
            query = query.Where(x => x.WebsiteUrl.Contains(parameters.WebsiteUrl));

        if (parameters.CreatedDate.HasValue)
            query = query.Where(x => x.CreatedDate == parameters.CreatedDate.Value.Date);

        if (parameters.LastUpdated.HasValue)
            query = query.Where(x => x.LastUpdated == parameters.LastUpdated.Value.Date);

        if (parameters.CreatedById.HasValue)
            query = query.Where(x => x.CreatedById == parameters.CreatedById.Value);

        if (parameters.UpdatedById.HasValue)
            query = query.Where(x => x.UpdatedById == parameters.UpdatedById.Value);

        var providers = await query.AsNoTracking().ToListAsync();
        return Ok(providers);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Post.Payload payload)
    {
        var provider = new Table
        {
            Id = Guid.NewGuid(),
            Name = payload.Name,
            Description = payload.Description,
            IconUrl = payload.IconUrl,
            WebsiteUrl = payload.WebsiteUrl,
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            CreatedById = Guid.NewGuid(),
        };
        _context.Providers.Add(provider);
        await _context.SaveChangesAsync();
        await _messageBus.PublishAsync(new Post.Messager.Message(provider.Id));
        await _hubContext.Clients.All.SendAsync("provider-created", provider.Id);
        return CreatedAtAction(nameof(Get), provider.Id);
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] Put.Payload payload)
    {
        var provider = await _context.Providers.FindAsync(payload.Id);
        if (provider == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Provider not found",
                Detail = $"Provider with ID {payload.Id} does not exist.",
                Status = StatusCodes.Status404NotFound,
                Instance = HttpContext.Request.Path
            });
        }
        provider.Name = payload.Name;
        provider.Description = payload.Description;
        provider.IconUrl = payload.IconUrl;
        provider.WebsiteUrl = payload.WebsiteUrl;
        provider.LastUpdated = DateTime.UtcNow;
        provider.UpdatedById = Guid.NewGuid();
        _context.Providers.Update(provider);
        await _context.SaveChangesAsync();
        await _messageBus.PublishAsync(new Put.Messager.Message(provider.Id));
        await _hubContext.Clients.All.SendAsync("provider-updated", provider.Id);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] Delete.Parameters parameters)
    {
        var provider = await _context.Providers.FindAsync(parameters.Id);
        if (provider == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Provider not found",
                Detail = $"Provider with ID {parameters.Id} does not exist.",
                Status = StatusCodes.Status404NotFound,
                Instance = HttpContext.Request.Path
            });
        }
        _context.Providers.Remove(provider);
        await _context.SaveChangesAsync();
        await _messageBus.PublishAsync(new Delete.Messager.Message(provider.Id));
        await _hubContext.Clients.All.SendAsync("provider-deleted", provider.Id);
        return NoContent();
    }
}
