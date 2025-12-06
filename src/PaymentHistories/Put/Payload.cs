using System.ComponentModel.DataAnnotations;

namespace Iowa.PaymentHistories.Put;

public class Payload
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid PackageId { get; set; }
    public Guid? DiscountId { get; set; }
    public string ChartColor { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string Currency { get; set; } = string.Empty;
}
