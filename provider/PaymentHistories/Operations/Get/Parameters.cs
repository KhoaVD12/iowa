namespace Provider.PaymentHistories.Get;

public class Parameters
{
    public Guid? Id { get; set; }
    public Guid? UserId { get; set; }
    public string? ChartColor { get; set; }
    public decimal? Price { get; set; }
    public string? Currency { get; set; }
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
}
