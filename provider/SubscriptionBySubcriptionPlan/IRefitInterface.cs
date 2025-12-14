using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Provider.SubscriptionBySubcriptionPlan;

public interface IRefitInterface
{
    [Get("/api/subscription_by_subscriptionPlan")]
    Task<ApiResponse<Model>> Get([Query] Get.Parameters parameters);

    [Post("/api/subscription_by_subscriptionPlan")]
    Task<ApiResponse<object>> Post([Body] Post.Payload payload);
}
