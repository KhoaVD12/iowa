namespace Iowa.Databases.TempDb;

public class Config
{
    public string ContactPoint { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Keyspace { get; set; } = string.Empty;
    public string DataCenter { get; set; } = string.Empty;
}
