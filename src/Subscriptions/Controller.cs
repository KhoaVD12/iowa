using Iowa.Databases.App;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Iowa.Subcriptions;

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
    public async Task<IActionResult> Get([FromQuery] Subscriptions.Get.Parameters parameters)
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
    public async Task<IActionResult> Post([FromBody] Subscriptions.Post.Payload payload)
    {
        var table = new Databases.App.Tables.Subcription.Table
        { };
        var existingPackage = await _context.Packages.FindAsync(payload.PackageId);

        if (existingPackage is null)
        {
            return NotFound($"Package with Id: {payload.PackageId} not found");
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

        _context.Subcriptions.Add(table);
        await _context.SaveChangesAsync();
        await _messageBus.PublishAsync(new Subscriptions.Post.Messager.Message(table.Id));
        return CreatedAtAction(nameof(Post), new { id = table.Id });
    }
    [HttpPut]
    public async Task<IActionResult> Put([FromBody] Subscriptions.Put.Payload payload)
    {
        var existSubscription = _context.Subcriptions.FirstOrDefault(x => x.Id == payload.Id);
        if (existSubscription == null)
        {
            return NotFound();
        }
        var existingPackage = await _context.Packages.FindAsync(payload.PackageId);

        if (existingPackage is null)
        {
            return NotFound($"Package with Id: {payload.PackageId} not found");
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

        _context.Subcriptions.Update(existSubscription);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] Subscriptions.Delete.Parameters parameters)
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
