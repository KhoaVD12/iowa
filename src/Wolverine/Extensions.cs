using Iowa.Databases.App;
using Microsoft.EntityFrameworkCore;

namespace Iowa.Wolverine;

public static class Extensions
{
    public static IServiceCollection AddDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IowaContext>(options =>
            options.UseSqlServer("Server=localhost;Database=Iowa;Trusted_Connection=True;TrustServerCertificate=True"));
        return services;
    }
}
