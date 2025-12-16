using Microsoft.Extensions.DependencyInjection;
using Provider.Packages;
using Provider.PaymentHistories;
using Provider.Providers;
using Provider.SubscriptionByUserIds;
using Provider.Subscriptions;
using Provider.UserIdBySubscriptionPlans;

namespace Provider;

public static class Extensions
{
    public static IServiceCollection AddProviders(this IServiceCollection services, Config config)
    {
        services.AddSingleton(config);
        services.AddTransient<MachineToken.Service>();
        services.RegisterPackages(config);
        services.RegisterSubscriptions(config);
        services.RegisterProviders(config);
        services.RegisterPaymentHistories(config);
        services.RegisterSubscriptionByUserIds(config);
        services.RegisterUserIdBySubscriptionPlans(config);
        return services;
    }
}
