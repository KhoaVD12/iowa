using Iowa.Databases.App;
using Azure;
using Iowa.Databases.App.Tables.Discount;
using Iowa.Models.PaginationResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
namespace Iowa.Discounts;
[Route("api/discounts")]
[ApiController]
public class Controller : ControllerBase
{
    private readonly IowaContext _context;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<Controller> _logger;
    private readonly IHubContext<Hub> _hubContext;
    public Controller(IowaContext context, IMessageBus messageBus, ILogger<Controller> logger, IHubContext<Hub> hubContext)
    {
        _context = context;
        _messageBus = messageBus;
        _logger = logger;
        _hubContext = hubContext;
    }
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Discounts.Get.Parameters parameters)
    {
        var query = _context.Discounts.AsQueryable();
        if (parameters.Id.HasValue)
        {
            query = query.Where(x => x.Id == parameters.Id.Value);
        }
        if (parameters.ProviderId.HasValue)
        {
            query = query.Where(x => x.ProviderId == parameters.ProviderId.Value);
        }
        if (!string.IsNullOrEmpty(parameters.Code))
        {
            query = query.Where(x => x.Code == parameters.Code);
        }
        if (!string.IsNullOrEmpty(parameters.DiscountType))
        {
            query = query.Where(x => x.DiscountType == parameters.DiscountType);
        }
        if (parameters.DiscountValue.HasValue)
        {
            query = query.Where(x => x.DiscountValue == parameters.DiscountValue.Value);
        }
        if (!string.IsNullOrEmpty(parameters.Description))
        {
            query = query.Where(x => x.Description == parameters.Description);
        }
        if (parameters.CreatedDate.HasValue)    
        {
            query = query.Where(x => x.CreatedDate == parameters.CreatedDate.Value);
        }
        if (parameters.CreatedById.HasValue)    
        {
            query = query.Where(x => x.CreatedById == parameters.CreatedById.Value);
        }
        if (parameters.LastUpdated.HasValue)    
        {
            query = query.Where(x => x.LastUpdated == parameters.LastUpdated.Value);
        }
        if (parameters.UpdatedById.HasValue)    
        {
            query = query.Where(x => x.UpdatedById == parameters.UpdatedById.Value);
        } 
        query = query.OrderByDescending(x => x.CreatedDate);
        var discounts = query.ToList();
        return Ok(discounts);
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Discounts.Post.Payload payload)
    {
        var table = new Databases.App.Tables.Discount.Table
        { };

        table.Id = Guid.NewGuid();
        table.ProviderId = payload.ProviderId;
        table.Code = payload.Code;
        table.DiscountType = payload.DiscountType;
        table.DiscountValue = payload.DiscountValue;
        table.Description = payload.Description;
        table.CreatedDate = DateTime.UtcNow;
        table.CreatedById = Guid.NewGuid();

        _context.Discounts.Add(table);
        await  _context.SaveChangesAsync();
        await _messageBus.PublishAsync(new Discounts.Post.Messager.Message(table.Id));
        return CreatedAtAction(nameof(Post), new { id = table.Id }, table);
    }
   [HttpPut]
    public async Task<IActionResult> Put([FromBody] Discounts.Put.Payload payload)
    {
       var table = _context.Discounts.FirstOrDefault(x => x.Id == payload.Id);
        if (table == null)
        {
            return NotFound();
        } 

        table.ProviderId = payload.ProviderId;
        table.Code = payload.Code;
        table.DiscountType = payload.DiscountType;
        table.DiscountValue = payload.DiscountValue;
        table.Description = payload.Description;  
        table.LastUpdated = DateTime.UtcNow;
        table.UpdatedById = Guid.NewGuid();      
        _context.Discounts.Update(table);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var table = _context.Discounts.FirstOrDefault(x => x.Id == id);
        if (table == null)
        {
            return NotFound();
        }

        _context.Discounts.Remove(table);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch([FromQuery]Guid id, [FromBody] JsonPatchDocument<Table> patchDoc,
                                    CancellationToken cancellationToken = default!)
    {
        var table = await _context.Discounts.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (table == null)
        {
            return NotFound();
        }

        patchDoc.ApplyTo(table);
        _context.Discounts.Update(table);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
        
}