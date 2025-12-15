using System.ComponentModel.DataAnnotations;

namespace Provider.PaymentHistories.Put;

public class Payload
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public Guid? DiscountId { get; set; }
    public string ChartColor { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string Currency { get; set; } = string.Empty;

}
