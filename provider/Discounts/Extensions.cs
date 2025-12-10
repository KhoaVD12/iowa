using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Provider.Discounts;

public static class Extensions
{
    public static void RegisterDiscounts(this IServiceCollection services, Config config)
    {
        services.AddTransient<RefitHttpClientHandler>();

        string baseUrl = config.Url;

        services.AddRefitClient<IRefitInterface>()
                .ConfigurePrimaryHttpMessageHandler<RefitHttpClientHandler>()
                .ConfigureHttpClient(x => x.BaseAddress = new Uri(baseUrl));
    }
}
