namespace Provider.Subscriptions.Post;

public class Payload
{
    public Guid UserId { get; set; }
    public DateTime PurchasedDate { get; set; }
    public Guid PackageId { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string ChartColor { get; set; } = string.Empty;
    public Guid? DiscountId { get; set; }
    public DateTime RenewalDate { get; set; }
    public bool IsRecursive { get; set; }
}
