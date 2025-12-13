using Cassandra;
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
            #region [CassandraDb Setting]
            var cassandraDbConfig = configuration.GetSection("CassandraDb").Get<TempDb.Config>();
            if (cassandraDbConfig != null)
            {
                Cluster cluster = Cluster.Builder()
                    .AddContactPoint(cassandraDbConfig.ContactPoint)
                    .WithPort(cassandraDbConfig.Port)
                    .Build();

                Cassandra.ISession session = cluster.Connect("subscriptions");
                services.AddSingleton<TempDb.TempContext>();
                services.AddSingleton(session);
            }
                #endregion

                services.AddDbContext<IowaContext>(options =>
                    options.UseSqlServer("Server=localhost;Database=Iowa;Trusted_Connection=True;TrustServerCertificate=True"));
            services.AddDbContext<IdentityContext>(options =>
                options.UseSqlServer("Server=localhost;Database=IowaIdentity;Trusted_Connection=True;TrustServerCertificate=True"));
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();
            return services;
        }
    }
}
