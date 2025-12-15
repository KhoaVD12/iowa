using Refit;
namespace Provider.Packages;

public interface IRefitInterface
{
    [Get("/api/packages")]
    Task<ApiResponse<Models.PaginationResults.Model<Model>>> GetAsync([Query] Get.Parameters parameters);

    [Post("/api/packages")]
    Task<ApiResponse<object>> PostAsync([Body] Post.Payload payload);

    [Put("/api/packages")]
    Task<ApiResponse<object>> PutAsync([Body] Put.Payload payload);

    [Delete("/api/packages")]
    Task<ApiResponse<object>> DeleteAsync([Query] Delete.Parameters parameters);

    [Patch("/api/packages")]
    Task<ApiResponse<object>> PatchAsync([Query] Patch.Parameters parameters, [Body] List<Patch.Operation> operations);
}
