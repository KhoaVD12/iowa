namespace Iowa.Subscriptions.Get;

public class Parameters
{
    public Guid? Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? ProviderId { get; set; }
    public Guid? PackageId { get; set; }
    public string? Currency { get; set; }
    public string ChartColor { get; set; } = string.Empty;
    public string? Status { get; set; }
}
