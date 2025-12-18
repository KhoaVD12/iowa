using System;
using System.Collections.Generic;
using System.Text;

namespace Provider.SubscriptionByUserIds.Delete;

public class Parameters
{
    public Guid UserId { get; set; }
    public string SubscriptionPlan { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
}
