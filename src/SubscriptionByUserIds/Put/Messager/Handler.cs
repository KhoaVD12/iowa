using Iowa.Databases.TempDb;
using Cassandra.Data.Linq;

namespace Iowa.SubscriptionByUserIds.Put.Messager;

public class Handler(TempContext context)
{
    private readonly TempContext _context = context;
    public async Task Handle(Message message)
    {
        await _context.UserIdBySubscriptionPlans.Where(x => x.SubscriptionPlan == message.OldSubscriptionPlan &&
                                                            x.CompanyName == message.OldCompanyName &&
                                                            x.UserId == message.OldUserId).Delete().ExecuteAsync();
        var userIdBySubscriptionPlan = new Databases.TempDb.Tables.UserIdBySubscriptionPlan.Table
        {
            Id = message.Id,
            UserId = message.NewUserId,
            SubscriptionPlan = message.NewSubscriptionPlan,
            CompanyName = message.NewCompanyName,
        };
        await _context.UserIdBySubscriptionPlans.Insert(userIdBySubscriptionPlan).ExecuteAsync();
    }
}
