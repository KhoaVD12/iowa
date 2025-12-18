using Iowa.Databases.TempDb;
using Microsoft.EntityFrameworkCore;
using Cassandra.Data.Linq;

namespace Iowa.SubscriptionByUserIds.Delete.Messager;

public class Handler(TempContext context)
{
    private readonly TempContext _context = context;
    public async Task Handle(Message message)
    {
        await _context.UserIdBySubscriptionPlans.Where(x => x.SubscriptionPlan == message.SubscriptionPlan &&
                                                            x.CompanyName == message.SubscriptionPlan &&
                                                            x.UserId == message.UserId).Delete().ExecuteAsync();
    }
}
