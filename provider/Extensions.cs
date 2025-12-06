using Microsoft.Extensions.DependencyInjection;
using Provider.Packages;

namespace Provider;

public static class Extensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Config config)
    {
        services.AddSingleton(config);
        services.AddTransient<MachineToken.Service>();
        services.RegisterSubscriptions(config);
        return services;
    }
}
