using System;
using System.Collections.Generic;
using System.Text;

namespace Provider.UserIdBySubscriptionPlans;

public class Model
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SubscriptionPlan { get; set; }
    public string CompanyName { get; set; }
}
