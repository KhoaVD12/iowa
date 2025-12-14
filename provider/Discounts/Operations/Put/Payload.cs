using System.ComponentModel.DataAnnotations;

namespace Provider.Discounts.Put;

public class Payload
{
    public Guid Id { get; set; }
    public Guid ProviderId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal? DiscountValue { get; set; }
    
}
