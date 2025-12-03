using Refit;

namespace Provider.BffSubscriptions;

public interface IRefitInterface
{
    [Get("/api/subscriptions/all")]
    Task<ApiResponse<Model>> All([Query] All.Parameters parameters);

}