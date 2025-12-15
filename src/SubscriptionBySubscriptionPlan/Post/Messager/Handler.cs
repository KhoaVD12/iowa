using Iowa.Databases.TempDb;

namespace Iowa.SubscriptionBySubscriptionPlan.Post.Messager;

public class Handler(TempContext context)
{
    private readonly TempContext _context = context;
    public async Task Handle(Message message)
    {
        var subscriptionPlanByUserId= new Databases.TempDb.Tables.SubscriptionByUserId.Table
        {
            Id = Guid.NewGuid(),
            UserId = message.UserId,
            SubscriptionPlan = message.SubscriptionPlan,
            SubscriptionBySubscriptionPlanId = message.SubscriptionPlanId,
        };
        await _context.SubscriptionByUserIds.Insert(subscriptionPlanByUserId).ExecuteAsync();
        Console.WriteLine($"Inserted SubscriptionByUserId for UserId: {message.UserId}, SubscriptionPlan: {message.SubscriptionPlan}");
    }
}
