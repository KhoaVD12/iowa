using Microsoft.Extensions.DependencyInjection;
using Provider.Packages;
using Provider.PaymentHistories;
using Provider.Providers;
using Provider.SubscriptionBySubcriptionPlan;
using Provider.Subscriptions;

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
        services.RegisterSubscriptionBySubscriptionPlan(config);
        return services;
    }
}
