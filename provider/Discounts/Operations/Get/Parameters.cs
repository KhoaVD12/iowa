namespace Provider.Discounts.Get;

public class Parameters
{
    public Guid? Id { get; set; }
    public Guid? ProviderId{ get; set; }
    public string? Code  { get; set; }    
    public string? DiscountType { get; set; } = string.Empty;
    public decimal? DiscountValue { get; set; }
    public string? Description { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public Guid? CreatedById { get; set; }
    public DateTime? LastUpdated { get; set; }
    public Guid? UpdatedById { get; set; }

}
