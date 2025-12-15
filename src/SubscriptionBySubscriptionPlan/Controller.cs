using Cassandra.Data.Linq;
using Iowa.Models.PaginationResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace Iowa.SubscriptionBySubscriptionPlan;

[Route("api/subscription_by_subscriptionPlan")]
[ApiController]
public class Controller : ControllerBase
{
    private readonly Databases.TempDb.TempContext _tempContext;
    private readonly IMessageBus _messageBus;
    public Controller(Databases.TempDb.TempContext tempContext,
        IMessageBus messageBus)
    {
        _tempContext = tempContext;
        _messageBus = messageBus;
    }
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Get.Parameters parameters)
    {
        var query = await _tempContext.SubscriptionBySubscriptionPlans.ExecuteAsync();

        if (parameters.Id.HasValue)
        {
            query = query.Where(m => m.Id == parameters.Id.Value);
        }
        if (parameters.UserId.HasValue)
        {
            query = query.Where(m => m.UserId == parameters.UserId.Value);
        }
        if (!string.IsNullOrEmpty(parameters.SubscriptionPlan))
        {
            query = query.Where(m => m.SubscriptionPlan == parameters.SubscriptionPlan);
        }
        if (!string.IsNullOrEmpty(parameters.CompanyName))
        {
            query = query.Where(m => m.CompanyName == parameters.CompanyName);
        }
        if (parameters.Price.HasValue)
        {
            query = query.Where(m => m.Price == parameters.Price.Value);
        }
        if (!string.IsNullOrEmpty(parameters.Currency))
        {
            query = query.Where(m => m.Currency == parameters.Currency);
        }
        if (!string.IsNullOrEmpty(parameters.ChartColor))
        {
            query = query.Where(m => m.ChartColor == parameters.ChartColor);
        }
        if (parameters.PurchasedDate.HasValue)
        {
            query = query.Where(m => m.PurchasedDate == parameters.PurchasedDate.Value);
        }
        if (parameters.RenewalDate.HasValue)
        {
            query = query.Where(m => m.RenewalDate == parameters.RenewalDate.Value);
        }
        if (parameters.IsRecusive)
        {
            query = query.Where(m => m.IsRecusive == parameters.IsRecusive);
        }

        if (parameters.PageSize.HasValue && parameters.PageIndex.HasValue && parameters.PageSize > 0 && parameters.PageIndex.Value >= 0)
            query = query.Skip(parameters.PageSize.Value * parameters.PageIndex.Value).Take(parameters.PageSize.Value); 

        var subscriptions = query.AsEnumerable();

        if (parameters.PageSize.HasValue && parameters.PageIndex.HasValue && parameters.PageSize > 0 && parameters.PageIndex.Value >= 0)
            subscriptions = subscriptions.Skip(parameters.PageSize.Value * parameters.PageIndex.Value).Take(parameters.PageSize.Value);

        return Ok(subscriptions);
    }
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Post.Payload payload)
    {
        var newRecord = new Databases.TempDb.Tables.SubscriptionBySubscriptionPlan.Table
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
        await _tempContext.SubscriptionBySubscriptionPlans.Insert(newRecord).ExecuteAsync();
        await _messageBus.PublishAsync(new Post.Messager.Message(newRecord.Id, newRecord.UserId, newRecord.SubscriptionPlan));
        return CreatedAtAction(nameof(Get), new { id = newRecord.Id });
    }
}
