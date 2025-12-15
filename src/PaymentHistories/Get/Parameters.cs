namespace Iowa.PaymentHistories.Get
{
    public class Parameters
    {
        public Guid? Id { get; set; }
        public Guid? UserId { get; set; }
        public string? ProviderName { get; set; } = string.Empty;
        public string? PackageName { get; set; } = string.Empty;
        public Guid? DiscountId { get; set; }
        public string? ChartColor { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public string? Currency { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public Guid? CreateById { get; set; }
        public Guid? UpdateById { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
    }
}
