using System;
using System.Collections.Generic;
using System.Text;

namespace Provider.SubscriptionByUserIds;

public class Model
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string SubscriptionPlan { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string ChartColor { get; set; } = string.Empty;
    public DateTime PurchasedDate { get; set; }
    public DateTime RenewalDate { get; set; }
    public bool IsRecursive { get; set; }
    public int TimesInMonth { get; set; }
    public bool UseCalendarMonthCycle { get; set; }
}
