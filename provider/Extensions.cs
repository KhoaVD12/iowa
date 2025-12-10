using Microsoft.Extensions.DependencyInjection;
using Provider.Packages;
using Provider.Providers;
using Provider.Subscriptions;
using Provider.Discounts;

namespace Provider;

public static class Extensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Config config)
    {
        services.AddSingleton(config);
        services.AddTransient<MachineToken.Service>();
        services.RegisterPackages(config);
        services.RegisterSubscriptions(config);
        services.RegisterProviders(config);
        services.RegisterDiscounts(config);
        return services;
    }
}
