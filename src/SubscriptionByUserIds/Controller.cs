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
            IsRecursive = payload.IsRecusive
        };

        await _tempContext.SubscriptionByUserIds.Insert(newRecord).ExecuteAsync();
        await _messageBus.PublishAsync(new Post.Messager.Message(newRecord.Id, newRecord.UserId, newRecord.SubscriptionPlan, newRecord.CompanyName));
        return CreatedAtAction(nameof(Get), new { id = newRecord.Id });
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] Put.Payload payload)
    {
        Databases.TempDb.Tables.SubscriptionByUserId.Table? existingRecord = await _tempContext.SubscriptionByUserIds.FirstOrDefault(x => x.UserId == payload.UserId &&
                                                                                               x.SubscriptionPlan == payload.SubscriptionPlan &&
                                                                                               x.CompanyName == payload.CompanyName).ExecuteAsync();
        if (existingRecord == null)
        {
            return NotFound();
        }
        var newRecord = new Databases.TempDb.Tables.SubscriptionByUserId.Table
        {
            Id = payload.Id,
            UserId = payload.UserId,
            SubscriptionPlan = payload.SubscriptionPlan,
            CompanyName = payload.CompanyName,
            Price = payload.Price,
            Currency = payload.Currency,
            ChartColor = payload.ChartColor,
            PurchasedDate = payload.PurchasedDate,
            RenewalDate = payload.RenewalDate,
            IsRecursive = payload.IsRecusive
        };

        await _tempContext.SubscriptionByUserIds.Insert(newRecord).ExecuteAsync();
        await _messageBus.PublishAsync(new Put.Messager.Message(newRecord.Id, newRecord.UserId, newRecord.SubscriptionPlan, newRecord.CompanyName));
        return CreatedAtAction(nameof(Get), new { id = newRecord.Id });
    }

    public async Task<IActionResult> Delete([FromQuery] Delete.Parameters parameters)
    {
        Databases.TempDb.Tables.SubscriptionByUserId.Table? existingRecord = await _tempContext.SubscriptionByUserIds.FirstOrDefault(x => x.UserId == parameters.UserId &&
                                                                                               x.SubscriptionPlan == parameters.SubscriptionPlan &&
                                                                                               x.CompanyName == parameters.CompanyName).ExecuteAsync();
        if(existingRecord == null)
        {
            return NotFound();
        }
        await _tempContext.SubscriptionByUserIds.Where(x => x.UserId == parameters.UserId &&
                                                            x.SubscriptionPlan == parameters.SubscriptionPlan &&
                                                            x.CompanyName == parameters.CompanyName).Delete().ExecuteAsync();
        await _messageBus.PublishAsync(new Delete.Messager.Message(existingRecord.SubscriptionPlan, existingRecord.CompanyName, existingRecord.UserId));
        return NoContent();
    }
}
