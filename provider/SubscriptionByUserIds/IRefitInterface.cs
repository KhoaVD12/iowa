using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Provider.SubscriptionByUserIds;

public interface IRefitInterface
{
    [Get("/api/subscription-by-user-ids")]
    Task<ApiResponse<ICollection<Model>>> GetAsync([Query] Get.Parameters parameters);

    [Post("/api/subscription-by-user-ids")]
    Task<ApiResponse<object>> Post([Body] Post.Payload payload);
}
