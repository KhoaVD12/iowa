using Iowa.Databases.App;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Provider;
using Provider.PaymentHistories.Patch;

namespace Test.PaymentHistories;

public class Test
{
    private readonly IServiceProvider serviceProvider;

    #region [ CTors ]

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
    #endregion

    #region [ Endpoints ]

    [Fact]
    public async Task GET_DataExist()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();
        var paymentHistory = new Iowa.Databases.App.Tables.PaymentHistory.Table
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ChartColor = "#333333",
            Price = 9.99m,
            Currency = "USD",
            CreatedDate = DateTime.UtcNow
        };
        dbContext.PaymentHistories.Add(paymentHistory); // <-- Correct: add to PaymentHistories DbSet
        await dbContext.SaveChangesAsync();
        var paymentHistoriesEndpoint = serviceProvider!.GetRequiredService<Provider.PaymentHistories.IRefitInterface>();
        var result = await paymentHistoriesEndpoint.Get(new()
        {
        });
        var items = result?.Content.Items;

        Assert.NotNull(result);
        Assert.NotNull(result.Content);
        Assert.Contains(items, p => p.Id == paymentHistory.Id);
        Assert.NotEqual(0, items.Count);
        dbContext.PaymentHistories.Remove(paymentHistory);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task POST_PaymentHistory()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();

        // Tạo provider trước

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

        dbContext.Packages.Add(package);
        await dbContext.SaveChangesAsync();

        Guid id = Guid.NewGuid();
        var PaymentHistoriesEndpoint = serviceProvider!.GetRequiredService<Provider.PaymentHistories.IRefitInterface>();

        var payload = new Provider.PaymentHistories.Post.Payload
        {
            UserId = Guid.NewGuid(),
            PackageId = package.Id,
            DiscountId = null,
            ChartColor = "#FFFFFF",
            Price = 9.99m,
            DiscountedPrice = null,
            Currency = "USD"
        };

        await PaymentHistoriesEndpoint.Post(payload);

        var expected = await dbContext.PaymentHistories.FirstOrDefaultAsync(p => p.UserId == payload.UserId);
        Assert.NotNull(expected);
        Assert.Equal(payload.UserId, expected.UserId);
        Assert.Equal("USD", expected.Currency);

        // Cleanup
        dbContext.PaymentHistories.Remove(expected);
        dbContext.Packages.Remove(package);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task PUT_PaymentHistory()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();

        // Tạo Package trước
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
        dbContext.Packages.Add(package);
        await dbContext.SaveChangesAsync();

        var paymentHistory = new Iowa.Databases.App.Tables.PaymentHistory.Table
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            PackageId = package.Id,
            DiscountId = null,
            ChartColor = "#FFFFFF",
            Price = 9.99m,
            DiscountedPrice = null,
            Currency = "USD",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        dbContext.PaymentHistories.Add(paymentHistory);
        await dbContext.SaveChangesAsync();

        var endpoint = serviceProvider!.GetRequiredService<Provider.PaymentHistories.IRefitInterface>();
        var payload = new Provider.PaymentHistories.Put.Payload
        {
            Id = paymentHistory.Id,
            UserId = Guid.NewGuid(),
            PackageId = package.Id,
            DiscountId = null,
            ChartColor = "#FFFF00",
            Price = 10.99m,
            DiscountedPrice = null,
            Currency = "VND"
        };

        await endpoint.Put(payload);

        await dbContext.Entry(paymentHistory).ReloadAsync();

        Assert.NotNull(paymentHistory);
        Assert.Equal("#FFFF00", paymentHistory.ChartColor);
        Assert.Equal("VND", paymentHistory.Currency);
        Assert.Equal(10.99m, paymentHistory.Price);

        // Cleanup
        dbContext.PaymentHistories.Remove(paymentHistory);
        dbContext.Packages.Remove(package);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task DELETE_PaymentHistory()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();

        var paymentHistory = new Iowa.Databases.App.Tables.PaymentHistory.Table
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            PackageId = Guid.NewGuid(),
            DiscountId = null,
            ChartColor = "#000000",
            Price = 9.99m,
            DiscountedPrice = null,
            Currency = "USD",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        dbContext.PaymentHistories.Add(paymentHistory);
        await dbContext.SaveChangesAsync();

        var endpoint = serviceProvider!.GetRequiredService<Provider.PaymentHistories.IRefitInterface>();
        await endpoint.Delete(new Provider.PaymentHistories.Delete.Parameters { Id = paymentHistory.Id });
        await dbContext.Entry(paymentHistory).ReloadAsync();
        var deleted = await dbContext.PaymentHistories.FindAsync(paymentHistory.Id);
        Assert.Null(deleted);

        
        // Cleanup provider
        //dbContext.Packages.Remove(package);
        //await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task PATCH_PaymentHistory()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();

        // Tạo Package trước
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
        dbContext.Packages.Add(package);
        await dbContext.SaveChangesAsync();

        var paymentHistory = new Iowa.Databases.App.Tables.PaymentHistory.Table
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            PackageId = package.Id,
            DiscountId = null,
            ChartColor = "#FFFFFF",
            Price = 9.99m,
            DiscountedPrice = null,
            Currency = "USD",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        dbContext.PaymentHistories.Add(paymentHistory);
        await dbContext.SaveChangesAsync();

        var endpoint = serviceProvider!.GetRequiredService<Provider.PaymentHistories.IRefitInterface>();
        var operations = new List<Operation>
    {
        new Operation { op = "replace", path = "/ChartColor", value = "#FFFF00" },
        new Operation { op = "replace", path = "/Currency", value = "VND" },
        new Operation { op = "replace", path = "/Price", value = 49.99m }
    };

        await endpoint.Patch(
            new Provider.PaymentHistories.Patch.Parameters
            {
                Id = paymentHistory.Id
            }, 
            operations
        );

        await dbContext.Entry(paymentHistory).ReloadAsync();

        Assert.NotNull(paymentHistory);
        Assert.Equal("#FFFF00", paymentHistory.ChartColor);
        Assert.Equal("VND", paymentHistory.Currency);
        Assert.Equal(49.99m, paymentHistory.Price);

        // Cleanup
        dbContext.Packages.Remove(package);
        dbContext.PaymentHistories.Remove(paymentHistory);
        await dbContext.SaveChangesAsync();
    }


    #endregion
}
