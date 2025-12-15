using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Provider.Subscriptions;

public interface IRefitInterface
{
    [Get("/api/subscriptions")]
    Task<ApiResponse<Models.PaginationResults.Model<Model>>> GetAsync([Query] Get.Parameters parameters);

    [Post("/api/subscriptions")]
    Task<ApiResponse<object>> PostAsync([Body] Post.Payload payload);

    [Put("/api/subscriptions")]
    Task<ApiResponse<object>> PutAsync([Body] Put.Payload payload);

    [Delete("/api/subscriptions")]
    Task<ApiResponse<object>> DeleteAsync([Query] Delete.Parameters parameters);

    [Patch("/api/subscriptions")]
    Task<ApiResponse<object>> PatchAsync([Query] Patch.Parameters parameters, [Body] List<Patch.Operation> operations);
}
