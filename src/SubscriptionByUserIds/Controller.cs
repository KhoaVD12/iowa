using Cassandra.Data.Linq;
using Iowa.Models.PaginationResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace Iowa.SubscriptionByUserIds;

[Route("api/subscription-by-user-ids")]
[Authorize]
[ApiController]
public class Controller : ControllerBase
{
    private readonly Databases.TempDb.TempContext _tempContext;
    private readonly IMessageBus _messageBus;
    public Controller(Databases.TempDb.TempContext tempContext, IMessageBus messageBus)
    {
        _tempContext = tempContext;
        _messageBus = messageBus;
    }
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Get.Parameters parameters)
    {
        var query = await _tempContext.SubscriptionByUserIds.ExecuteAsync();

        if (parameters.UserId.HasValue)
        {
            query = query.Where(m => m.UserId == parameters.UserId);
        }
        if (!string.IsNullOrEmpty(parameters.SubscriptionPlan))
        {
            query = query.Where(m => m.SubscriptionPlan == parameters.SubscriptionPlan);
        }
        if (!string.IsNullOrEmpty(parameters.CompanyName))
        {
            query = query.Where(m => m.CompanyName == parameters.CompanyName);
        }
        var subscriptions = query.AsEnumerable();

        return Ok(subscriptions);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Post.Payload payload)
    {
        var newRecord = new Databases.TempDb.Tables.SubscriptionByUserId.Table
        {
            Id = Guid.NewGuid(),
            UserId = payload.UserId,
            SubscriptionPlan = payload.SubscriptionPlan,
            CompanyName = payload.CompanyName,
            Price = payload.Price,
            Currency = payload.Currency,
            ChartColor = payload.ChartColor,
            PurchasedDate = payload.PurchasedDate,
            RenewalDate = payload.RenewalDate,
            IsRecusive = payload.IsRecusive
        };

        await _tempContext.SubscriptionByUserIds.Insert(newRecord).ExecuteAsync();
        await _messageBus.PublishAsync(new Post.Messager.Message(newRecord.UserId, newRecord.SubscriptionPlan, newRecord.CompanyName));
        return CreatedAtAction(nameof(Get), new { id = newRecord.Id });
    }
}
