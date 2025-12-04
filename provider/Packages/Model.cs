namespace Provider.Packages;
public class Model
{
    public Guid Id { get; set; }
    public Guid ProviderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastUpdated { get; set; }
    public Guid CreatedById { get; set; }
    public Guid? UpdatedById { get; set; }
}

