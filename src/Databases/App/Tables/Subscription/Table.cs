using System;
using System.Runtime.InteropServices;

namespace Iowa.Databases.App.Tables.Subscription;

public class Table
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PackageId { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string ChartColor { get; set; } = string.Empty;
    public Guid? DiscountId { get; set; }
    public DateTime RenewalDate { get; set; }
    public bool IsRecursive { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid CreatedById { get; set; }
    public DateTime? LastUpdated { get; set; }
    public Guid? UpdatedById { get; set; }
}
