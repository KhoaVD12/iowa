using Iowa.Databases.App;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Iowa.Subscriptions;

[Route("api/subscriptions")]
[ApiController]
public class Controller : ControllerBase
{
    private readonly IowaContext _context;
    private readonly IMessageBus _messageBus;
    public Controller(IowaContext context, IMessageBus messageBus)
    {
        _context = context;
        _messageBus = messageBus;
    }
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Get.Parameters parameters)
    {
        var query = _context.Subcriptions.AsQueryable();

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

        var subscriptions = query.ToList();
        return Ok(subscriptions);
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Post.Payload payload)
    {
        var table = new Databases.App.Tables.Subcription.Table
        { };

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

        _context.Subcriptions.Add(table);
        await _context.SaveChangesAsync();
        await _messageBus.PublishAsync(new Post.Messager.Message(table.Id));
        return CreatedAtAction(nameof(Post), new { id = table.Id });
    }
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] Put.Payload payload)
    {
        var table = _context.Subcriptions.FirstOrDefault(x => x.Id == payload.Id);
        if (table == null)
        {
            return NotFound();
        }

        table.UserId = payload.UserId;
        table.ProviderId = payload.ProviderId;
        table.PackageId = payload.PackageId;
        table.Price =  payload.Price;
        table.DiscountedPrice = payload.DiscountedPrice;
        table.Currency = payload.Currency;
        table.ChartColor = payload.ChartColor;
        table.DiscountId = payload.DiscountId;
        table.RenewalDate = payload.RenewalDate;
        table.LastUpdated = DateTime.UtcNow;
        table.UpdatedById = payload.UserId;

        _context.Subcriptions.Update(table);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] Delete.Parameters parameters)
    {
        var table = _context.Subcriptions.FirstOrDefault(x => x.Id == parameters.Id);
        if (table == null)
        {
            return NotFound();
        }
        _context.Subcriptions.Remove(table);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
