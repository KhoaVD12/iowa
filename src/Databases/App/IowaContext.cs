using Microsoft.EntityFrameworkCore;

namespace Iowa.Databases.App;

public class IowaContext : DbContext
{
    public IowaContext(DbContextOptions<IowaContext> options)
        : base(options)
    {
    }
    public DbSet<Tables.Subcription.Table> Subcriptions { get; set; }
    public DbSet<Tables.Package.Table> Packages { get; set; }
    public DbSet<Tables.PaymentHistory.Table> PaymentHistories { get; set; }
    public DbSet<Tables.Discount.Table> Discounts { get; set; }
}
