using Iowa.Databases.App;
using Iowa.Models.PaginationResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System.Security.Claims;
using Wolverine;

namespace Iowa.PaymentHistories;

[Authorize]
[Route("api/payment-histories")]
[ApiController]
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
    [AllowAnonymous]
    public async Task<IActionResult> Get([FromQuery] Get.Parameters parameters)
    {
        var query = _context.PaymentHistories.AsQueryable();

        if (parameters.Id.HasValue) 
        { 
            query = query.Where(x => x.Id == parameters.Id.Value);
        }
        if (parameters.UserId.HasValue) 
        { 
            query = query.Where(x => x.UserId == parameters.UserId.Value);
        }
        if (!string.IsNullOrEmpty(parameters.ChartColor)) 
        { 
            query = query.Where(x => x.ChartColor == parameters.ChartColor);
        }
        if (parameters.Price.HasValue) 
        { 
            query = query.Where(x => x.Price == parameters.Price.Value);
        }
        if (!string.IsNullOrEmpty(parameters.Currency)) 
        { 
            query = query.Where(x => x.Currency == parameters.Currency);
        }

        query = query.OrderByDescending(x => x.CreatedDate);
        if (parameters.PageIndex.HasValue && parameters.PageSize.HasValue) 
        { 
            var skip = parameters.PageIndex.Value * parameters.PageSize.Value;
            query = query.Skip(skip).Take(parameters.PageSize.Value);
        }
        var paymentHistories = await query.AsNoTracking().ToListAsync();
        var paginationResults = new Builder<Databases.App.Tables.PaymentHistory.Table>()
            .WithAll(await query.CountAsync())
            .WithIndex(parameters.PageIndex ?? 0)
            .WithSize(parameters.PageSize ?? paymentHistories.Count)
            .WithTotal(paymentHistories.Count())
            .WithItems(paymentHistories)
            .Build();
        return Ok(paginationResults);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Post.Payload payload)
    {
        var table = new Databases.App.Tables.PaymentHistory.Table();

        table.Id = Guid.NewGuid();
        table.UserId = payload.UserId;
        table.PackageId = payload.PackageId;
        table.DiscountId = payload.DiscountId;
        table.ChartColor = payload.ChartColor;
        table.Price = payload.Price;
        table.DiscountedPrice = payload.DiscountedPrice;
        table.Currency = payload.Currency;
        //table.PaymentDate = DateTime.UtcNow;
        table.CreatedDate = DateTime.UtcNow;
        table.CreateById = payload.UserId; // Replace with actual user ID

        //var existingPackage = await _context.Packages.FindAsync(payload.PackageId);
        
        //if (existingPackage is null)
        //{
        //    return NotFound(new ProblemDetails
        //    {
        //        Title = "Package not found",
        //        Detail = $"Package with ID {payload.PackageId} does not exist.",
        //        Status = StatusCodes.Status404NotFound,
        //        Instance = HttpContext.Request.Path
        //    });   
        //}
        

        _context.PaymentHistories.Add(table);
        await _context.SaveChangesAsync();
        await _messageBus.PublishAsync(new Post.Messager.Message(table.Id));
        await _hubContext.Clients.All.SendAsync("PaymentHistory-Created", table.Id);
        return CreatedAtAction(nameof(Post), new { id = table.Id }, table);
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] Put.Payload payload)
    {
        var existPaymentHistory = _context.PaymentHistories.FirstOrDefault(x => x.Id == payload.Id);
        if (existPaymentHistory == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "PaymentHistory not found",
                Detail = $"PaymentHistory with ID {payload.Id} does not exist.",
                Status = StatusCodes.Status404NotFound,
                Instance = HttpContext.Request.Path
            });
        }

        existPaymentHistory.UserId = payload.UserId;
        existPaymentHistory.PackageId = payload.PackageId;
        existPaymentHistory.DiscountId = payload.DiscountId;
        existPaymentHistory.ChartColor = payload.ChartColor;
        existPaymentHistory.Price = payload.Price;
        existPaymentHistory.DiscountedPrice = payload.DiscountedPrice;
        existPaymentHistory.Currency = payload.Currency;
        existPaymentHistory.LastUpdated =  DateTime.UtcNow;

        //var existingPackage = await _context.Packages.FindAsync(payload.PackageId);

        //if (existingPackage is null)
        //{
        //    return NotFound(new ProblemDetails
        //    {
        //        Title = "Package not found",
        //        Detail = $"Package with ID {payload.PackageId} does not exist.",
        //        Status = StatusCodes.Status404NotFound,
        //        Instance = HttpContext.Request.Path
        //    });   
        //}


        _context.PaymentHistories.Update(existPaymentHistory);
        await _context.SaveChangesAsync();
        await _messageBus.PublishAsync(new Put.Messager.Message(payload.Id));
        await _hubContext.Clients.All.SendAsync("PaymentHistory-Updated", payload.Id);
        return NoContent();
    }

    [HttpPatch]
    public async Task<IActionResult> Patch([FromQuery] Guid id, [FromBody] JsonPatchDocument<Databases.App.Tables.PaymentHistory.Table> patchDoc,
                                   CancellationToken cancellationToken = default!)
    {
        //if (User.Identity is null)
        //    return Unauthorized();

        //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //if (userId is null)
        //    return Unauthorized("User Id not found");

        var changes = new List<(string Path, object? Value)>();
        foreach (var op in patchDoc.Operations)
        {
            if (op.OperationType != OperationType.Replace && op.OperationType != OperationType.Test)
                return BadRequest("Only Replace and Test operations are allowed in this patch request.");
            changes.Add((op.path, op.value));
        }

        if (patchDoc is null)
            return BadRequest("Patch document cannot be null.");

        var entity = await _context.PaymentHistories.FindAsync(id, cancellationToken);
        if (entity == null)
            return NotFound(new ProblemDetails
            {
                Title = "PaymentHistory not found",
                Detail = $"PaymentHistory with ID {id} does not exist.",
                Status = StatusCodes.Status404NotFound,
                Instance = HttpContext.Request.Path
            });

        patchDoc.ApplyTo(entity);

        entity.LastUpdated = DateTime.UtcNow;

        _context.PaymentHistories.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);


        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] Delete.Parameters parameters)
    {
        var existPaymentHistory = _context.PaymentHistories.FirstOrDefault(x => x.Id == parameters.Id);
        if (existPaymentHistory == null)
        {
            return NotFound(new ProblemDetails
            {
                Title = "PaymentHistory not found",
                Detail = $"PaymentHistory with ID {parameters.Id} does not exist.",
                Status = StatusCodes.Status404NotFound,
                Instance = HttpContext.Request.Path
            });
        }
        _context.PaymentHistories.Remove(existPaymentHistory);
        await _context.SaveChangesAsync();
        await _messageBus.PublishAsync(new Delete.Messager.Message(parameters.Id));
        await _hubContext.Clients.All.SendAsync("PaymentHistory-Deleted", parameters.Id);
        return NoContent();
    }
}
