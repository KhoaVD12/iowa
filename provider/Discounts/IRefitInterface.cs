using Refit;
namespace Provider.Discounts;

public interface IRefitInterface
{
    [Get("/api/discounts")]
    Task<ApiResponse<Models.PaginationResults.Model<Model>>> Get([Query] Get.Parameters parameters);

    [Post("/api/discounts")]
    Task<ApiResponse<object>> Post([Body] Post.Payload payload);

    [Put("/api/discounts")]
    Task<ApiResponse<object>> Put([Body] Put.Payload payload);

    [Delete("/api/discounts")]
    Task<ApiResponse<object>> Delete([Query] Delete.Parameters parameters);

    [Patch("/api/discounts")]
    Task<ApiResponse<object>> Patch([Query] Patch.Parameters parameters, [Body] List<Patch.Operation> operations);
}
