using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Provider.Subscriptions;

public interface IRefitInterface
{
    [Get("/api/subscriptions")]
    Task<ApiResponse<Models.PaginationResults.Model<Model>>> Get([Query] Get.Parameters parameters);

    [Post("/api/subscriptions")]
    Task<ApiResponse<object>> Post([Body] Post.Payload payload);

    [Put("/api/subscriptions")]
    Task<ApiResponse<object>> Put([Body] Put.Payload payload);

    [Delete("/api/subscriptions")]
    Task<ApiResponse<object>> Delete([Query] Delete.Parameters parameters);

    [Patch("/api/subscriptions")]
    Task<ApiResponse<object>> Patch([Query] Patch.Parameters parameters, [Body] List<Patch.Operation> operations);
}
