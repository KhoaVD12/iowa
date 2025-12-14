using Cassandra.Data.Linq;
using Iowa.Models.PaginationResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Iowa.SubscriptionBySubscriptionPlan;

[Route("api/subscription_by_subscriptionPlan")]
[ApiController]
public class Controller : ControllerBase
{
    private readonly Databases.TempDb.TempContext _tempContext;
    public Controller(Databases.TempDb.TempContext tempContext)
    {
        _tempContext = tempContext;
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
        if (!string.IsNullOrEmpty(parameters.Company))
        {
            query = query.Where(m => m.Company == parameters.Company);
        }
        if (parameters.Price.HasValue)
        {
            query = query.Where(m => m.Price == parameters.Price.Value);
        }
        if (parameters.RenewalDate.HasValue)
        {
            query = query.Where(m => m.RenewalDate == parameters.RenewalDate.Value);
        }
        if (parameters.IsRecusive)
        {
            query = query.Where(m => m.IsRecusive == parameters.IsRecusive);
        }
        if (parameters.CompanyId.HasValue)
        {
            query = query.Where(m => m.CompanyId == parameters.CompanyId.Value);
        }

        if (parameters.PageSize.HasValue && parameters.PageIndex.HasValue && parameters.PageSize > 0 && parameters.PageIndex.Value >= 0)
            query = query.Skip(parameters.PageSize.Value * parameters.PageIndex.Value).Take(parameters.PageSize.Value); 

        var subscriptions = query.ToList();

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
            Company = payload.Company,
            Price = payload.Price,
            RenewalDate = payload.RenewalDate,
            IsRecusive = false,
            CompanyId = null
        };
        await _tempContext.SubscriptionBySubscriptionPlans.Insert(newRecord).ExecuteAsync();
        return CreatedAtAction(nameof(Get), new { id = newRecord.Id });
    }
}
