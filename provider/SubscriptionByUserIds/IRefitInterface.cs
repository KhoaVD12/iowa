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
    Task<ApiResponse<object>> PostAsync([Body] Post.Payload payload);

    [Post("/api/subscription-by-user-ids")]
    Task<ApiResponse<object>> PutAsync([Body] Put.Payload payload);

    [Delete("/api/subscription-by-user-ids")]
    Task<ApiResponse<object>> DeleteAsync([Query] Delete.Parameters parameters);
}
