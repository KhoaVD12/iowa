using System.ComponentModel.DataAnnotations;

namespace Iowa.Providers.Put;

public class Payload
{
    [Required]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
}
