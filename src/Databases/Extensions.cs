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
            #region Cassandra
            var cassandraDbConfig = configuration.GetSection("CassandraDb").Get<TempDb.Config>();
            if (cassandraDbConfig != null)
            {
                try
                {
                    var cluster = Cluster.Builder()
                        .AddContactPoint(cassandraDbConfig.ContactPoint)
                        .WithPort(cassandraDbConfig.Port)
                        .WithLoadBalancingPolicy(new DCAwareRoundRobinPolicy(cassandraDbConfig.DataCenter))
                        .Build();

                    // Nếu chưa có keyspace, có thể Connect() không tham số trước
                    Cassandra.ISession session;
                    if (!string.IsNullOrEmpty(cassandraDbConfig.Keyspace))
                    {
                        session = cluster.Connect(cassandraDbConfig.Keyspace);
                    }
                    else
                    {
                        session = cluster.Connect();
                    }

                    services.AddSingleton<TempDb.TempContext>();
                    services.AddSingleton(session);

                    Console.WriteLine("✅ Cassandra connected");
                }
                catch (Cassandra.NoHostAvailableException)
                {
                    Console.WriteLine("⚠️ Cassandra not available, skipping...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Cassandra error: {ex.Message}, skipping...");
                }
            }
            #endregion

            var iowaDbConfig = configuration.GetSection("IowaDb").Get<Sql.DbConfig>();
            var identityDbConfig = configuration.GetSection("IdentityDb").Get<Sql.DbConfig>();

            if (iowaDbConfig == null)
            {
                throw new ArgumentNullException(nameof(iowaDbConfig), "IowaDb configuration section is missing or invalid.");
            }
            if (identityDbConfig == null)
            {
                throw new ArgumentNullException(nameof(identityDbConfig), "IdentityDb configuration section is missing or invalid.");
            }
            var iowaConnectionString = new Sql.ConnectionStringBuilder()
                .WithHost(iowaDbConfig.Host)
                .WithPort(iowaDbConfig.Port)
                .WithDatabase(iowaDbConfig.Database)
                .WithUsername(iowaDbConfig.Username)
                .WithPassword(iowaDbConfig.Password)
                //.WithTrustedConnection()
                .WithTrustServerCertificate()
                .Build();

            var identityConnectionString = new Sql.ConnectionStringBuilder()
                .WithHost(identityDbConfig.Host)
                .WithPort(identityDbConfig.Port)
                .WithDatabase(identityDbConfig.Database)
                .WithUsername(identityDbConfig.Username)
                .WithPassword(identityDbConfig.Password)
                //.WithTrustedConnection()
                .WithTrustServerCertificate()
                .Build();

            services.AddDbContext<IowaContext>(x =>
            {
                x.EnableSensitiveDataLogging();
                x.UseSqlServer(iowaConnectionString, sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null));
            });

            services.AddDbContext<IdentityContext>(x =>
                x.UseSqlServer(identityConnectionString, sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null)));

            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<IdentityContext>()
                    .AddDefaultTokenProviders();
            return services;
        }
    }
}
