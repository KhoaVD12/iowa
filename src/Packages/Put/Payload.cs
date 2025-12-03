using System.ComponentModel.DataAnnotations;

namespace Iowa.Packages.Put;

public class Payload
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public Guid ProviderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string Currency { get; set; } = string.Empty;

}
