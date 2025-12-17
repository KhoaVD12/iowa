using Iowa.Databases.TempDb;
using Cassandra.Data.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Iowa.Databases.App;

namespace Iowa.Packages.Post.Messager;

public class Handler(TempContext tempContext, IowaContext context)
{
    private readonly IowaContext _context = context;
    private readonly TempContext _tempContext = tempContext;
    public async Task Handle(Message message)
    {
        var UserIds = await _tempContext.UserIdBySubscriptionPlans.Where(x => x.SubscriptionPlan == message.SubscriptionPlan &&
                                                                          x.CompanyName == message.CompanyName).Select(x => x.UserId).ExecuteAsync();
        foreach (var userId in UserIds)
        {
            var casSubscriptions = await _tempContext.SubscriptionByUserIds.Where(x => x.UserId == userId &&
                                                            x.SubscriptionPlan == message.SubscriptionPlan &&
                                                            x.CompanyName == message.CompanyName).ExecuteAsync();
            var subscriptions = casSubscriptions.Select(x => new Databases.App.Tables.Subscription.Table
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        PackageId = message.PackageId,
                        Price = x.Price,
                        Currency = x.Currency,
                        ChartColor = x.ChartColor,
                        RenewalDate = x.RenewalDate,
                        PurchasedDate = x.PurchasedDate,
                        IsRecursive = x.IsRecursive,
                        CreatedDate = DateTime.UtcNow,
                        CreatedById = Guid.Empty
                    }).ToList();

            _context.AddRange(subscriptions);
            await _context.SaveChangesAsync();
            await _tempContext.SubscriptionByUserIds.Where(x => x.UserId == userId &&
                                                            x.SubscriptionPlan == message.SubscriptionPlan &&
                                                            x.CompanyName == message.CompanyName).Delete().ExecuteAsync();
        }

        await _tempContext.UserIdBySubscriptionPlans.Where(x => x.SubscriptionPlan == message.SubscriptionPlan &&
                                                       x.CompanyName == message.CompanyName).Delete().ExecuteAsync();
    }
}
