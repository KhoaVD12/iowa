namespace Iowa.Subscriptions.Get;

public class Parameters
{
    public Guid? Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? ProviderId { get; set; }
    public Guid? PackageId { get; set; }
    public string? Currency { get; set; }
    public string ChartColor { get; set; } = string.Empty;
    public bool? Status { get; set; }
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
    public bool? IsRecursive { get; set; }
}
