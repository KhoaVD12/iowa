using Refit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Provider.UserIdBySubscriptionPlans;

public interface IRefitInterface
{
    [Get("/api/user-id-by-subscription-plans")]
    Task<ApiResponse<ICollection<Model>>> GetAsync([Query] Get.Parameters parameters);
}
