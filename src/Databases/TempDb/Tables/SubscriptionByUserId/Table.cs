using Cassandra.Mapping.Attributes;

namespace Iowa.Databases.TempDb.Tables.SubscriptionByUserId;

[Table("subscription_by_userid")]
public class Table
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } 
    public string SubscriptionPlan { get; set; } = string.Empty;
    public Guid SubscriptionBySubscriptionPlanId { get; set; }

}
