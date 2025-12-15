using Refit;
namespace Provider.PaymentHistories;

public interface IRefitInterface
{
    [Get("/api/payment-histories")]
    Task<ApiResponse<Models.PaginationResults.Model<Model>>> GetAsync([Query] Get.Parameters parameters);

    [Post("/api/payment-histories")]
    Task<ApiResponse<object>> PostAsync([Body] Post.Payload payload);

    [Put("/api/payment-histories")]
    Task<ApiResponse<object>> PutAsync([Body] Put.Payload payload);

    [Delete("/api/payment-histories")]
    Task<ApiResponse<object>> DeleteAsync([Query] Delete.Parameters parameters);

    [Patch("/api/payment-histories")]
    Task<ApiResponse<object>> PatchAsync([Query] Patch.Parameters parameters, [Body] List<Patch.Operation> operations);
}
