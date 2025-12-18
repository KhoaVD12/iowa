namespace Provider.SubscriptionByUserIds.Get;

public class Parameters
{
    public Guid? UserId { get; set; }
    public string? SubscriptionPlan { get; set; } = string.Empty;
    public string? CompanyName { get; set; } = string.Empty;
}
