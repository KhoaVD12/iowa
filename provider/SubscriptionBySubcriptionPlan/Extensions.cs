using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Provider.SubscriptionBySubcriptionPlan;

public static class Extensions
{
    public static void RegisterSubscriptionBySubscriptionPlan(this IServiceCollection services, Config config)
    {
        services.AddTransient<RefitHttpClientHandler>();

        string baseUrl = config.Url;

        services.AddRefitClient<IRefitInterface>()
                .ConfigurePrimaryHttpMessageHandler<RefitHttpClientHandler>()
                .ConfigureHttpClient(x => x.BaseAddress = new Uri(baseUrl));
    }
}
