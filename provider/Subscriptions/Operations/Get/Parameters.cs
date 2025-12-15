using System;
using System.Collections.Generic;
using System.Text;

namespace Provider.Subscriptions.Get;

public class Parameters
{
    public Guid? Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? ProviderId { get; set; }
    public Guid? PackageId { get; set; }
    public string? Currency { get; set; }
    public string? ChartColor { get; set; }
    public bool? Status { get; set; }
    public bool? IsRecursive { get; set; }
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public decimal? Price { get; set; }
    public string? Ids { get; set; }
    public string? SearchTerm { get; set; }
    public string? Include { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? LastUpdated { get; set; }
    public Guid? CreatedById { get; set; }
    public Guid? UpdatedById { get; set; }
}
