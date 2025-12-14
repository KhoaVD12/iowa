namespace Iowa.SubscriptionBySubscriptionPlan.Get;

public class Parameters
{
    public Guid? Id { get; set; }
    public Guid? UserId { get; set; }
    public string? SubscriptionPlan { get; set; } = string.Empty;
    public string? Company { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public DateTime? RenewalDate { get; set; }
    public bool IsRecusive { get; set; } = false;
    public Guid? CompanyId { get; set; }
    public int? PageSize { get; set; }
    public int? PageIndex { get; set; }
}
