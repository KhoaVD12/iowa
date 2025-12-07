using Refit;
namespace Provider.Providers;

public interface IRefitInterface
{
    [Get("/api/providers")]
    Task<ApiResponse<Models.PaginationResults.Model<Model>>> Get([Query] Get.Parameters parameters);

    [Post("/api/providers")]
    Task<ApiResponse<object>> Post([Body] Post.Payload payload);

    [Put("/api/providers")]
    Task<ApiResponse<object>> Put([Body] Put.Payload payload);

    [Delete("/api/providers")]
    Task<ApiResponse<object>> Delete([Query] Delete.Parameters parameters);

    [Patch("/api/providers")]
    Task<ApiResponse<object>> Patch([Query] Patch.Parameters parameters, [Body] List<Patch.Operation> operations);
}
