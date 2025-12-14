namespace Iowa.SubscriptionBySubscriptionPlan.Post;

public class Payload
{
    public Guid UserId { get; set; }
    public string SubscriptionPlan { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime RenewalDate { get; set; }
}
