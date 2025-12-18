namespace Iowa.SubscriptionByUserIds.Put;

public class Payload
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SubscriptionPlan { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string ChartColor { get; set; } = string.Empty;
    public DateTime PurchasedDate { get; set; }
    public DateTime RenewalDate { get; set; }
    public bool IsRecusive { get; set; }
}
