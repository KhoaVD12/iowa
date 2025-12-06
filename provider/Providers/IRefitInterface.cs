using Provider.Providers.Operations.Patch;
using Refit;

namespace Provider.Providers;

public interface IRefitInterface
{
    [Get("/api/providers")]
    Task<ApiResponse<Models.PaginationResults.Model<Model>>> Get([Query] Operations.Get.Parameters parameters);

    [Post("/api/providers")]
    Task<ApiResponse<object>> Post([Body] Operations.Post.Payload payload);

    [Put("/api/providers")]
    Task<ApiResponse<object>> Put([Body] Operations.Put.Payload payload);

    [Delete("/api/providers")]
    Task<ApiResponse<object>> Delete([Query] Operations.Delete.Parameters parameters);

    [Patch("/api/providers")]
    Task<ApiResponse<object>> Patch([Query] Operations.Patch.Parameters parameters, [Body] List<Operation> operations);
}
