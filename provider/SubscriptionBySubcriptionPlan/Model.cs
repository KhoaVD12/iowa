using System;
using System.Collections.Generic;
using System.Text;

namespace Provider.SubscriptionBySubcriptionPlan;

public class Model
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SubscriptionPlan { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime RenewalDate { get; set; }
    public bool IsRecusive { get; set; }
    public Guid? CompanyId { get; set; }
}
