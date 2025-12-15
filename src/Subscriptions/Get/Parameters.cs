namespace Iowa.Subscriptions.Get;

public class Parameters
{
    public Guid? Id { get; set; }
    public Guid? UserId { get; set; }
    public DateTime? PurchasedDate { get; set; }
    public Guid? PackageId { get; set; }
    public decimal? Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string? Currency { get; set; }
    public string? ChartColor { get; set; } = string.Empty;
    public bool IsRecursive { get; set; } =false;
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
}
