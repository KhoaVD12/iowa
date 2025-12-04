namespace Iowa.Databases.App.Tables.Provider;

public class Table
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconUrl { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastUpdated { get; set; }
    public Guid CreatedById { get; set; }
    public Guid? UpdatedById { get; set; }
}