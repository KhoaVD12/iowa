using Cassandra.Mapping.Attributes;

namespace Iowa.Databases.TempDb.Tables.SubscriptionByUserId;

[Table("subscription_by_user_id")]
public class Table
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SubscriptionPlan { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string ChartColor { get; set; } = string.Empty;
    public DateTime RenewalDate { get; set; }
    public bool IsRecursive { get; set; }
}
