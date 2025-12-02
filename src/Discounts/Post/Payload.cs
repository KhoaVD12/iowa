using System.ComponentModel.DataAnnotations;
namespace Iowa.Discounts.Post;
public class Payload
{
    [Required]
    public Guid ProviderId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public string Description { get; set; } = string.Empty;

}