using Microsoft.Extensions.DependencyInjection;
using Provider.BffSubscriptions;

namespace Provider;

public static class Extensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Config config)
    {
        services.AddSingleton(config);
        services.AddSingleton<Token.Service>();
        services.RegisterSubscriptions(config);
        return services;
    }
}
