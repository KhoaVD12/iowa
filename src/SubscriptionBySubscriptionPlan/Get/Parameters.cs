namespace Iowa.SubscriptionBySubscriptionPlan.Get;

public class Parameters
{
    public Guid? Id { get; set; }
    public Guid? UserId { get; set; }
    public string? SubscriptionPlan { get; set; } = string.Empty;
    public string? CompanyName { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? Currency { get; set; }
    public string? ChartColor { get; set; }
    public DateTime? PurchasedDate { get; set; }
    public DateTime? RenewalDate { get; set; }
    public bool IsRecusive { get; set; } = false;
    public Guid? CompanyId { get; set; }
    public int? PageSize { get; set; }
    public int? PageIndex { get; set; }
}
