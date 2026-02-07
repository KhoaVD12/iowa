using Cassandra;
using Iowa.Databases.App;
using Iowa.Databases.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Iowa.Databases
{
    public static class Extensions
    {
        public static IServiceCollection AddDatabases(this IServiceCollection services, IConfiguration configuration)
        {
            #region Cassandra
            int attempts = 0;
            var cassandraDbConfig = configuration.GetSection("CassandraDb").Get<TempDb.Config>();
            while (attempts < 5)
            {
                try
                {
                    var builder = Cluster.Builder()
                        .AddContactPoint(cassandraDbConfig.ContactPoint)
                        .WithPort(cassandraDbConfig.Port)
                        .WithLoadBalancingPolicy(new DCAwareRoundRobinPolicy(cassandraDbConfig.DataCenter));

                    // Nếu có username/password thì thêm vào
                    if (!string.IsNullOrEmpty(cassandraDbConfig.Username) &&
                        !string.IsNullOrEmpty(cassandraDbConfig.Password))
                    {
                        builder = builder.WithCredentials(cassandraDbConfig.Username, cassandraDbConfig.Password);
                    }

                    var cluster = builder.Build();

                    Cassandra.ISession session;
                    if (!string.IsNullOrEmpty(cassandraDbConfig.Keyspace))
                    {
                        session = cluster.Connect(cassandraDbConfig.Keyspace);
                    }
                    else
                    {
                        session = cluster.Connect();
                    }

                    services.AddSingleton<Cassandra.ISession>(session);

                    services.AddSingleton<TempDb.TempContext>(provider =>
                    {
                        var s = provider.GetRequiredService<Cassandra.ISession>();
                        return new TempDb.TempContext(s);
                    });

                    Console.WriteLine("Cassandra connected!!");
                    break;
                }
                catch (Exception ex)
                {
                    attempts++;
                    Console.WriteLine($"⚠️ Cassandra connection attempt {attempts} failed due to: {ex.Message}");
                }
            }


            //if (cassandraDbConfig != null)
            //{
            //    try
            //    {
            //        var cluster = Cluster.Builder()
            //            .AddContactPoint(cassandraDbConfig.ContactPoint)
            //            .WithPort(cassandraDbConfig.Port)
            //            .WithLoadBalancingPolicy(new DCAwareRoundRobinPolicy(cassandraDbConfig.DataCenter))
            //            .Build();

            //        // Nếu chưa có keyspace, có thể Connect() không tham số trước
            //        Cassandra.ISession session;
            //        if (!string.IsNullOrEmpty(cassandraDbConfig.Keyspace))
            //        {
            //            session = cluster.Connect(cassandraDbConfig.Keyspace);
            //        }
            //        else
            //        {
            //            session = cluster.Connect();
            //        }

            //        services.AddSingleton<TempDb.TempContext>();
            //        services.AddSingleton(session);

            //        Console.WriteLine("✅ Cassandra connected");
            //    }
            //    catch (Cassandra.NoHostAvailableException)
            //    {
            //        Console.WriteLine("⚠️ Cassandra not available, skipping...");
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"⚠️ Cassandra error: {ex.Message}, skipping...");
            //    }
            //}
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
