using Cassandra.Mapping.Attributes;

namespace Iowa.Databases.TempDb.Tables.UserIdBySubscriptionPlan;

[Table("user_id_by_subscription_plan")]
public class Table
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SubscriptionPlan { get; set; }
    public string CompanyName { get; set; }
}
