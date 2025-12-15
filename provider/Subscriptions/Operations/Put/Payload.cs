using System.ComponentModel.DataAnnotations;

namespace Provider.Subscriptions.Put;

public class Payload
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid PackageId { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string ChartColor { get; set; } = string.Empty;
    public DateTime RenewalDate { get; set; }
    public bool Status { get; set; }

}
