using Cassandra.Data.Linq;
using Iowa.Models.PaginationResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Iowa.SubscriptionByUserId;

[Route("api/subscription_by_userId")]
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
        var query = await _tempContext.SubscriptionByUserIds.ExecuteAsync();

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
        if (parameters.SubscriptionBySubscriptionPlanId.HasValue)
        {
            query = query.Where(m => m.Id == parameters.SubscriptionBySubscriptionPlanId.Value);
        }
        if (parameters.PageSize.HasValue && parameters.PageIndex.HasValue && parameters.PageSize > 0 && parameters.PageIndex.Value >= 0)
            query = query.Skip(parameters.PageSize.Value * parameters.PageIndex.Value).Take(parameters.PageSize.Value); 

        var subscriptions = query.AsEnumerable();

        if (parameters.PageSize.HasValue && parameters.PageIndex.HasValue && parameters.PageSize > 0 && parameters.PageIndex.Value >= 0)
            subscriptions = subscriptions.Skip(parameters.PageSize.Value * parameters.PageIndex.Value).Take(parameters.PageSize.Value);

        return Ok(subscriptions);
    }
}
