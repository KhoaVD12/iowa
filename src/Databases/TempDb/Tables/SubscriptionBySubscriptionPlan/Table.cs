using Cassandra.Mapping.Attributes;

namespace Iowa.Databases.TempDb.Tables.SubscriptionBySubscriptionPlan;

[Table("subscription_by_subscriptionPlan")]
public class Table
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
