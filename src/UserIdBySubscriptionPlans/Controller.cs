using Cassandra.Data.Linq;
using Iowa.Models.PaginationResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wolverine;

namespace Iowa.UserIdBySubscriptionPlans;

[Route("api/user-id-by-subscription-plans")]
[Authorize]
[ApiController]
public class Controller : ControllerBase
{
    private readonly Databases.TempDb.TempContext _tempContext;
    public Controller(Databases.TempDb.TempContext tempContext,
        IMessageBus messageBus)
    {
        _tempContext = tempContext;
    }
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Get.Parameters parameters)
    {
        var query = await _tempContext.UserIdBySubscriptionPlans.ExecuteAsync();
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
}
