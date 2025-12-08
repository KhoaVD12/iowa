using Refit;
namespace Provider.PaymentHistories;

public interface IRefitInterface
{
    [Get("/api/payment-histories")]
    Task<ApiResponse<Models.PaginationResults.Model<Model>>> Get([Query] Get.Parameters parameters);

    [Post("/api/payment-histories")]
    Task<ApiResponse<object>> Post([Body] Post.Payload payload);

    [Put("/api/payment-histories")]
    Task<ApiResponse<object>> Put([Body] Put.Payload payload);

    [Delete("/api/payment-histories")]
    Task<ApiResponse<object>> Delete([Query] Delete.Parameters parameters);

    [Patch("/api/payment-histories")]
    Task<ApiResponse<object>> Patch([Query] Patch.Parameters parameters, [Body] List<Patch.Operation> operations);
}
