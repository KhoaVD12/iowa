namespace Iowa.Databases.App.Tables.Discount;

public class Table
{
    public Guid Id { get; set; }
    public Guid ProviderId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public decimal DiscountValue { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime? LastUpdated { get; set; }
    public Guid? UpdatedById { get; set; }
}
/*
0. Id
1. ProviderId
2. code (varchar: SUMMER2024, NEWYEAR50...)
3. DiscountType (enum: Percentage, FixedAmount)
4. DiscountValue (decimal: 20 cho 20%, hoặc 50000 cho giảm 50k)
5. Description
6. CreatedDate
7. LastUpdated (Nullable)
8. CreatedById
9. UpdatedById (Nullable)
*/