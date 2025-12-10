using Iowa.Databases.App;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Provider;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Subscriptions;

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
        services.AddEndpoints(providerConfig);
        services.AddDbContext<IowaContext>(options =>
                options.UseSqlServer("Server=localhost;Database=Iowa;Trusted_Connection=True;TrustServerCertificate=True"));
        this.serviceProvider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task DELETE_Subscription()
    {
        var context = serviceProvider.GetRequiredService<IowaContext>();
        var provider = new Iowa.Databases.App.Tables.Provider.Table
        {
            Id = Guid.NewGuid(),
            Name = "TEST PROVIDER",
            Description = "Provider for DELETE test",
            IconUrl = "https://example.com/provider.png",
            WebsiteUrl = "https://example.com",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            CreatedById = Guid.NewGuid()
        };
        var package = new Iowa.Databases.App.Tables.Package.Table
        {
            Id = Guid.NewGuid(),
            ProviderId = Guid.NewGuid(),
            Name = "TEST PACKAGE",
            Description = "A basic TEST PACKAGE for testing purposes.",
            IconUrl = "https://example.com/icon.png",
            Price = 9.99m,
            Currency = "USD",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };
        var subscription = new Iowa.Databases.App.Tables.Subscription.Table
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ProviderId = provider.Id,
            PackageId = package.Id,
            Price = 9.99m,
            DiscountedPrice = null,
            Currency = "USD",
            RenewalDate = DateTime.UtcNow.AddMonths(1),
            Status = "Active",
            ChartColor = "#FF0000",
            CreatedDate = DateTime.UtcNow,
            CreatedById = Guid.NewGuid()
        };
        context.Providers.Add(provider);
        context.Packages.Add(package);
        context.Subscriptions.Add(subscription);
        await context.SaveChangesAsync();

        var subscriptionEndpoint = serviceProvider!.GetRequiredService<Provider.Subscriptions.IRefitInterface>();

        var deleteParameters = new Provider.Subscriptions.Delete.Parameters
        {
            Id = subscription.Id,
        };
        var result = await subscriptionEndpoint.Delete(deleteParameters);

        await context.Entry(subscription).ReloadAsync();
        var deletedSubscription = await context.Packages.FindAsync(subscription.Id);

        Assert.Null(deletedSubscription);
        context.Providers.Remove(provider);
        context.Packages.Remove(package);
        await context.SaveChangesAsync();
    }
    [Fact]
    //GET test
    public async Task GET_Subscriptions()
    {
        var context = serviceProvider.GetRequiredService<IowaContext>();
        var subscription = new Iowa.Databases.App.Tables.Subscription.Table
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ProviderId = Guid.NewGuid(),
            PackageId = Guid.NewGuid(),
            Price = 9.99m,
            DiscountedPrice = null,
            Currency = "USD",
            RenewalDate = DateTime.UtcNow.AddMonths(1),
            Status = "Active",
            ChartColor = "#FF0000",
            CreatedDate = DateTime.UtcNow,
            CreatedById = Guid.NewGuid()
        };
        context.Subscriptions.Add(subscription);
        await context.SaveChangesAsync();
        var subscriptionEndpoint = serviceProvider!.GetRequiredService<Provider.Subscriptions.IRefitInterface>();
        var getParameters = new Provider.Subscriptions.Get.Parameters
        {
            Id= subscription.Id,
        };
        var result = await subscriptionEndpoint.Get(getParameters);
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(result.Content);
        Assert.True(result.Content?.Items?.Count <= 10);
    }
}
