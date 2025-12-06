using Iowa.Databases.App;
using Iowa.Databases.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Iowa.Databases
{
    public static class Extensions
    {
        public static IServiceCollection AddDatabases(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IowaContext>(options =>
                options.UseSqlServer("Server=.\\SQLEXPRESS;Database=IowaDb;Trusted_Connection=True;TrustServerCertificate=True"));
            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer("Server=.\\SQLEXPRESS;Database=IowaIdentity;Trusted_Connection=True;TrustServerCertificate=True"));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();
            return services;
        }
    }
}
