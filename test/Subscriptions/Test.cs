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
        services.AddProviders(providerConfig);
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
            Status = true,
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
        var result = await subscriptionEndpoint.DeleteAsync(deleteParameters);

        await context.Entry(subscription).ReloadAsync();
        var deletedSubscription = await context.Packages.FindAsync(subscription.Id);

        Assert.Null(deletedSubscription);
        context.Providers.Remove(provider);
        context.Packages.Remove(package);
        await context.SaveChangesAsync();
    }
    [Fact]
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
            Status = true,
            ChartColor = "#FF0000",
            CreatedDate = DateTime.UtcNow,
            CreatedById = Guid.NewGuid()
        };
        context.Subscriptions.Add(subscription);
        await context.SaveChangesAsync();
        var subscriptionEndpoint = serviceProvider!.GetRequiredService<Provider.Subscriptions.IRefitInterface>();
        var result = await subscriptionEndpoint.GetAsync(new()
        {
            
        });
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(result.Content);
        Assert.NotEqual(0, result.Content?.Items?.Count);
    }
    [Fact]
    public async Task POST_Subscription()
    {
        var context = serviceProvider.GetRequiredService<IowaContext>();
        var provider = new Iowa.Databases.App.Tables.Provider.Table
        {
            Id = Guid.NewGuid(),
            Name = "TEST PROVIDER",
            Description = "Provider for POST test",
            IconUrl = "https://example.com/provider.png",
            WebsiteUrl = "https://example.com",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            CreatedById = Guid.NewGuid()
        };
        var package = new Iowa.Databases.App.Tables.Package.Table
        {
            Id = Guid.NewGuid(),
            ProviderId = provider.Id,
            Name = "TEST PACKAGE",
            Description = "A basic TEST PACKAGE for testing purposes.",
            IconUrl = "https://example.com/icon.png",
            Price = 9.99m,
            Currency = "USD",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };
        context.Providers.Add(provider);
        context.Packages.Add(package);
        await context.SaveChangesAsync();
        var subscriptionEndpoint = serviceProvider!.GetRequiredService<Provider.Subscriptions.IRefitInterface>();
        var subscription = new Provider.Subscriptions.Post.Payload
        {
            UserId = Guid.NewGuid(),
            ProviderId = provider.Id,
            PackageId = package.Id,
            Price = 9.99m,
            Currency = "USD",
            RenewalDate = DateTime.UtcNow.AddMonths(1),
            ChartColor = "#00FF00",
            Status = true
        };
        var result = await subscriptionEndpoint.PostAsync(subscription);
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
        Assert.NotNull(result.Content);
        var createdSubscription = await context.Subscriptions.FirstOrDefaultAsync(p=>p.UserId== subscription.UserId);
        Assert.Equal(subscription.UserId, createdSubscription?.UserId);
        Assert.Equal(subscription.ProviderId, createdSubscription?.ProviderId);
        Assert.Equal(subscription.PackageId, createdSubscription?.PackageId);
        Assert.Equal(subscription.Price, createdSubscription?.Price);
        Assert.Equal(subscription.Currency, createdSubscription?.Currency);
        Assert.Equal(subscription.RenewalDate, createdSubscription?.RenewalDate);
        context.Providers.Remove(provider);
        context.Packages.Remove(package);
        if (createdSubscription != null)
        {
            context.Subscriptions.Remove(createdSubscription);
        }
        await context.SaveChangesAsync();
    }
    [Fact]
    public async Task PUT_Subscription()
    {
        var context = serviceProvider.GetRequiredService<IowaContext>();
        var oldUserId = Guid.NewGuid();
        var newUserId = Guid.NewGuid();
        var provider = new Iowa.Databases.App.Tables.Provider.Table
        {
            Id = Guid.NewGuid(),
            Name = "TEST PROVIDER",
            Description = "Provider for POST test",
            IconUrl = "https://example.com/provider.png",
            WebsiteUrl = "https://example.com",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            CreatedById = Guid.NewGuid()
        };
        var package = new Iowa.Databases.App.Tables.Package.Table
        {
            Id = Guid.NewGuid(),
            ProviderId = provider.Id,
            Name = "TEST PACKAGE",
            Description = "A basic TEST PACKAGE for testing purposes.",
            IconUrl = "https://example.com/icon.png",
            Price = 9.99m,
            Currency = "USD",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };
        Guid subscriptionId= Guid.NewGuid();
        var subscription = new Iowa.Databases.App.Tables.Subscription.Table
        {
            Id = subscriptionId,
            UserId = oldUserId,
            ProviderId = provider.Id,
            PackageId = package.Id,
            Price = 9.99m,
            DiscountedPrice = null,
            Currency = "USD",
            RenewalDate = DateTime.UtcNow.AddMonths(1),
            Status = true,
            ChartColor = "#FF0000",
            CreatedDate = DateTime.UtcNow,
            CreatedById = Guid.NewGuid()
        };
        context.Packages.Add(package);
        context.Providers.Add(provider);
        context.Subscriptions.Add(subscription);
        await context.SaveChangesAsync();
        var subscriptionEndpoint = serviceProvider!.GetRequiredService<Provider.Subscriptions.IRefitInterface>();
        var updatedPayload = new Provider.Subscriptions.Put.Payload
        {
            Id = subscription.Id,
            UserId = newUserId,
            ProviderId = subscription.ProviderId,
            PackageId = subscription.PackageId,
            Price = 19.99m,
            Currency = "USD",
            ChartColor = "#0000FF",
            RenewalDate = DateTime.UtcNow.AddMonths(2),
            Status = false
        };
        var result = await subscriptionEndpoint.PutAsync(updatedPayload);
        await context.Entry(subscription).ReloadAsync();
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
        var updatedSubscription = await context.Subscriptions.FindAsync(subscriptionId);
        Assert.Equal(updatedPayload.Price, updatedSubscription?.Price);
        Assert.Equal(updatedPayload.UserId, updatedSubscription?.UserId);
        Assert.Equal(updatedPayload.RenewalDate, updatedSubscription?.RenewalDate);
        context.Subscriptions.Remove(subscription);
        await context.SaveChangesAsync();
    }
    [Fact]
    //PATCH
    public async Task PATCH_Subscription()
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
            Status = true,
            ChartColor = "#FF0000",
            CreatedDate = DateTime.UtcNow,
            CreatedById = Guid.NewGuid()
        };
        context.Subscriptions.Add(subscription);
        await context.SaveChangesAsync();

        Guid newUserId= Guid.NewGuid();
        decimal newPrice= 49.99m;

        var subscriptionEndpoint = serviceProvider!.GetRequiredService<Provider.Subscriptions.IRefitInterface>();
        var operations = new List<Provider.Subscriptions.Patch.Operation>
    {
        new Provider.Subscriptions.Patch.Operation { op = "replace", path = "/UserId", value = newUserId },
        new Provider.Subscriptions.Patch.Operation { op = "replace", path = "/Price", value = newPrice }
    };
        var result = await subscriptionEndpoint.PatchAsync(new Provider.Subscriptions.Patch.Parameters {Id=subscription.Id }, operations);
        await context.Entry(subscription).ReloadAsync();
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
        var patchedSubscription = await context.Subscriptions.FindAsync(subscription.Id);
        Assert.Equal(newPrice, patchedSubscription?.Price);
        Assert.Equal(newUserId, patchedSubscription?.UserId);
        context.Subscriptions.Remove(subscription);
        await context.SaveChangesAsync();
    }
}
