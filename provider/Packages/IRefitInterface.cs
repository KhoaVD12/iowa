using Refit;
namespace Provider.Packages;

public interface IRefitInterface
{
    [Get("/api/packages")]
    Task<ApiResponse<Models.PaginationResults.Model<Model>>> Get([Query] Get.Parameters parameters);

    [Post("/api/packages")]
    Task<ApiResponse<object>> Post([Body] Post.Payload payload);

    [Put("/api/packages")]
    Task<ApiResponse<object>> Put([Body] Put.Payload payload);

    [Delete("/api/packages")]
    Task<ApiResponse<object>> Delete([Query] Delete.Parameters parameters);

    [Patch("/api/packages")]
    Task<ApiResponse<object>> Patch([Query] Patch.Parameters parameters, [Body] List<Patch.Operation> operations);
}
