using Iowa.Databases.TempDb;

namespace Iowa.SubscriptionByUserIds.Post.Messager;

public class Handler(TempContext context)
{
    private readonly TempContext _context = context;
    public async Task Handle(Message message)
    {
        var userIdBySubscriptionPlan = new Databases.TempDb.Tables.UserIdBySubscriptionPlan.Table
        {
            Id = message.Id,
            UserId = message.UserId,
            SubscriptionPlan = message.SubscriptionPlan,
            CompanyName = message.CompanyName,
        };
        await _context.UserIdBySubscriptionPlans.Insert(userIdBySubscriptionPlan).ExecuteAsync();
    }
}
