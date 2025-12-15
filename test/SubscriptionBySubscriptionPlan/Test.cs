using Cassandra;
using Iowa.Databases.App;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Provider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Test.SubscriptionBySubscriptionPlan;

public class Test
{
    private readonly IServiceProvider serviceProvider;
    public Test()
    {
        var providerConfig = new Config(
                    url: "https://localhost:7063/",
                    secretKey: "secretKey"
                );
        var services = new ServiceCollection();
        services.AddProviders(providerConfig);
        Cluster cluster = Cluster.Builder()
                    .AddContactPoint("localhost")
                    .WithPort(9042)
                    .Build();

        Cassandra.ISession session = cluster.Connect("subscriptions");
        services.AddSingleton<Iowa.Databases.TempDb.TempContext>();
        services.AddSingleton(session);
        this.serviceProvider = services.BuildServiceProvider();
    }
    [Fact]
    public async Task GET_GetSubscriptionBySubscriptionPlan()
    {
        var tempContext = serviceProvider.GetRequiredService<Iowa.Databases.TempDb.TempContext>();
        //Add sample data
        var subscriptionBySubscriptionPlan = new Iowa.Databases.TempDb.Tables.SubscriptionBySubscriptionPlan.Table
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Price = 9.99m,
            SubscriptionPlan = "Basic",
            RenewalDate = DateTime.UtcNow.AddMonths(1),
            CompanyName = "Test Company",
            IsRecusive = true
        };
        await tempContext.SubscriptionBySubscriptionPlans.Insert(subscriptionBySubscriptionPlan).ExecuteAsync();
        var endpoint = serviceProvider.GetRequiredService<Provider.SubscriptionBySubcriptionPlan.IRefitInterface>();
        var subscriptions = await endpoint.Get(new());
        Assert.NotNull(subscriptions);
        Assert.True(subscriptions.IsSuccessStatusCode);
    }
}
