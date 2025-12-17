namespace Iowa.SubscriptionByUserIds.Delete;

public class Parameters
{
    public List<Guid> UserIds { get; set; } = new List<Guid>();
    public string SubscriptionPlan { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
}
