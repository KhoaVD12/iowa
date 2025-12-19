using Iowa.Databases.TempDb;
using Cassandra.Data.Linq;

namespace Iowa.SubscriptionByUserIds.Delete.Messager;

public class Handler(TempContext context)
{
    private readonly TempContext _context = context;
    public async Task Handle(Message message)
    {
        var check = await _context.UserIdBySubscriptionPlans.Where(x => x.SubscriptionPlan == message.SubscriptionPlan &&
                                                            x.CompanyName == message.CompanyName &&
                                                            x.UserId == message.UserId).ExecuteAsync();

        await _context.UserIdBySubscriptionPlans.Where(x => x.SubscriptionPlan == message.SubscriptionPlan &&
                                                            x.CompanyName == message.CompanyName &&
                                                            x.UserId == message.UserId).Delete().ExecuteAsync();
    }
}
