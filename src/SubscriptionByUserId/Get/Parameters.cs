namespace Iowa.SubscriptionByUserId.Get;

public class Parameters
{
    public Guid? Id { get; set; }
    public Guid? UserId { get; set; }
    public string? SubscriptionPlan { get; set; } = string.Empty;
    public Guid? SubscriptionBySubscriptionPlanId { get; set; }
    public int? PageSize { get; set; }
    public int? PageIndex { get; set; }
}
