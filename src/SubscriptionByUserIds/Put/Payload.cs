namespace Iowa.SubscriptionByUserIds.Put;

public class Payload
{
    public Guid Id { get; set; }
    public Guid OldUserId { get; set; }
    public string OldSubscriptionPlan { get; set; } = string.Empty;
    public string OldCompanyName { get; set; } = string.Empty;
    public Guid NewUserId { get; set; }
    public string NewSubscriptionPlan { get; set; } = string.Empty;
    public string NewCompanyName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string ChartColor { get; set; } = string.Empty;
    public DateTime PurchasedDate { get; set; }
    public DateTime RenewalDate { get; set; }
    public bool IsRecursive { get; set; }
    public int TimesInMonth { get; set; }
    public bool UseCalendarMonthCycle { get; set; }
}
