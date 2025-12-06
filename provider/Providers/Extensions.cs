using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Provider.Providers;

public static class Extensions
{
    public static void RegisterProviders(this IServiceCollection services, Config config)
    {
        services.AddTransient<RefitHttpClientHandler>();

        string baseUrl = config.Url;

        services.AddRefitClient<IRefitInterface>()
                .ConfigurePrimaryHttpMessageHandler<RefitHttpClientHandler>()
                .ConfigureHttpClient(x => x.BaseAddress = new Uri(baseUrl));
    }
}
