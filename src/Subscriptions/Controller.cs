using Iowa.Databases.App;
using Iowa.Models.PaginationResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Wolverine;

namespace Iowa.Subscriptions;

[Route("api/subscriptions")]
[Authorize]
[ApiController]
public class Controller : ControllerBase
{
    private readonly IowaContext _context;
    private readonly IMessageBus _messageBus;
    private readonly IHubContext<Hub> _hubContext;
    public Controller(IowaContext context, IMessageBus messageBus, IHubContext<Hub> hubContext)
    {
        _context = context;
        _messageBus = messageBus;
        _hubContext = hubContext;
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] Get.Parameters parameters)
    {
        var query = _context.Subscriptions.AsQueryable();

        if (parameters.Id.HasValue)
        {
            query = query.Where(x => x.Id == parameters.Id.Value);
        }
        if (parameters.UserId.HasValue)
        {
            query = query.Where(x => x.UserId == parameters.UserId.Value);
        }
        if (parameters.ProviderId.HasValue)
        {
            query = query.Where(x => x.ProviderId == parameters.ProviderId.Value);
        }
        if (parameters.PackageId.HasValue)
        {
            query = query.Where(x => x.PackageId == parameters.PackageId.Value);
        }
        if (!string.IsNullOrEmpty(parameters.Currency))
        {
            query = query.Where(x => x.Currency == parameters.Currency);
        }
        if (!string.IsNullOrEmpty(parameters.Status))
        {
            query = query.Where(x => x.Status == parameters.Status);
        }
        if (!string.IsNullOrEmpty(parameters.ChartColor))
        {
            query = query.Where(x => x.ChartColor == parameters.ChartColor);
        }
        query = query.OrderByDescending(x => x.CreatedDate);
        if (parameters.PageSize.HasValue && parameters.PageIndex.HasValue && parameters.PageSize > 0 && parameters.PageIndex.Value >= 0)
            query = query.Skip(parameters.PageSize.Value * parameters.PageIndex.Value).Take(parameters.PageSize.Value);

        var subscriptions = await query.AsNoTracking().ToListAsync();
        var paginationResults = new Builder<Databases.App.Tables.Subscription.Table>()
         .WithAll(await query.CountAsync())
         .WithIndex(parameters.PageIndex)
         .WithSize(parameters.PageSize)
         .WithTotal(subscriptions.Count)
         .WithItems(subscriptions)
         .Build();

        return Ok(paginationResults);
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Post.Payload payload)
    {
        var table = new Databases.App.Tables.Subscription.Table
        { };
        var existingPackage = await _context.Packages.FindAsync(payload.PackageId);

        if (existingPackage is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Package not found",
                Detail = $"Package with ID {payload.PackageId} does not exist.",
                Status = StatusCodes.Status404NotFound,
                Instance = HttpContext.Request.Path
            });
        }

        table.Id = Guid.NewGuid();
        table.UserId = payload.UserId;
        table.ProviderId = payload.ProviderId;
        table.PackageId = payload.PackageId;
        table.Price =  payload.Price;
        table.DiscountedPrice = payload.DiscountedPrice;
        table.Currency = payload.Currency;
        table.ChartColor = payload.ChartColor;
        table.DiscountId = payload.DiscountId;
        table.RenewalDate = payload.RenewalDate;
        table.Status = "active";
        table.CreatedDate = DateTime.UtcNow;
        table.CreatedById = payload.UserId;

        _context.Subscriptions.Add(table);
        await _context.SaveChangesAsync();
        await _messageBus.PublishAsync(new Post.Messager.Message(table.Id));
        await _hubContext.Clients.All.SendAsync("subscription-created", table.Id);
        return CreatedAtAction(nameof(Post), new { id = table.Id });
    }
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] Put.Payload payload)
    {
        var existSubscription = _context.Subscriptions.FirstOrDefault(x => x.Id == payload.Id);
        if (existSubscription == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Subscription not found",
                Detail = $"Subscription with ID {payload.Id} does not exist.",
                Status = StatusCodes.Status404NotFound,
                Instance = HttpContext.Request.Path
            });
        }
        var existingPackage = await _context.Packages.FindAsync(payload.PackageId);

        if (existingPackage is null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Package not found",
                Detail = $"Package with ID {payload.PackageId} does not exist.",
                Status = StatusCodes.Status404NotFound,
                Instance = HttpContext.Request.Path
            });
        }

        existSubscription.UserId = payload.UserId;
        existSubscription.ProviderId = payload.ProviderId;
        existSubscription.PackageId = payload.PackageId;
        existSubscription.Price =  payload.Price;
        existSubscription.DiscountedPrice = payload.DiscountedPrice;
        existSubscription.Currency = payload.Currency;
        existSubscription.ChartColor = payload.ChartColor;
        existSubscription.DiscountId = payload.DiscountId;
        existSubscription.RenewalDate = payload.RenewalDate;
        existSubscription.LastUpdated = DateTime.UtcNow;
        existSubscription.UpdatedById = payload.UserId;

        _context.Subscriptions.Update(existSubscription);
        await _context.SaveChangesAsync();
        await _messageBus.PublishAsync(new Put.Messager.Message(payload.Id));
        await _hubContext.Clients.All.SendAsync("subscription-updated", payload.Id);
        return NoContent();
    }

    [HttpPatch]
    public async Task<IActionResult> Patch([FromQuery] Guid id,
                                   [FromBody] JsonPatchDocument<Databases.App.Tables.Subscription.Table> patchDoc,
                                   CancellationToken cancellationToken = default!)
    {
        if (User.Identity is null)
            return Unauthorized();

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null)
            return Unauthorized("User Id not found");

        var changes = new List<(string Path, object? Value)>();
        foreach (var op in patchDoc.Operations)
        {
            if (op.OperationType != OperationType.Replace && op.OperationType != OperationType.Test)
                return BadRequest("Only Replace and Test operations are allowed in this patch request.");
            changes.Add((op.path, op.value));
        }

        if (patchDoc is null)
            return BadRequest("Patch document cannot be null.");

        var entity = await _context.Subscriptions.FindAsync(id, cancellationToken);
        if (entity == null)
            return NotFound(new ProblemDetails
            {
                Title = "Subscription not found",
                Detail = $"Subscription with ID {id} does not exist.",
                Status = StatusCodes.Status404NotFound,
                Instance = HttpContext.Request.Path
            });

        patchDoc.ApplyTo(entity);

        entity.LastUpdated = DateTime.UtcNow;

        _context.Subscriptions.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] Delete.Parameters parameters)
    {
        var table = _context.Subscriptions.FirstOrDefault(x => x.Id == parameters.Id);
        if (table == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Subscription not found",
                Detail = $"Subscription with ID {parameters.Id} does not exist.",
                Status = StatusCodes.Status404NotFound,
                Instance = HttpContext.Request.Path
            });
        }
        _context.Subscriptions.Remove(table);
        await _context.SaveChangesAsync();
        await _messageBus.PublishAsync(new Delete.Messager.Message(parameters.Id));
        await _hubContext.Clients.All.SendAsync("subscription-deleted", parameters.Id);
        return NoContent();
    }
}
