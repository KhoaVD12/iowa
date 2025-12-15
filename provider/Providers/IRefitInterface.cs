using Refit;
namespace Provider.Providers;

public interface IRefitInterface
{
    [Get("/api/providers")]
    Task<ApiResponse<Models.PaginationResults.Model<Model>>> GetAsync([Query] Get.Parameters parameters);

    [Post("/api/providers")]
    Task<ApiResponse<object>> PostAsync([Body] Post.Payload payload);

    [Put("/api/providers")]
    Task<ApiResponse<object>> PutAsync([Body] Put.Payload payload);

    [Delete("/api/providers")]
    Task<ApiResponse<object>> DeleteAsync([Query] Delete.Parameters parameters);

    [Patch("/api/providers")]
    Task<ApiResponse<object>> PatchAsync([Query] Patch.Parameters parameters, [Body] List<Patch.Operation> operations);
}
