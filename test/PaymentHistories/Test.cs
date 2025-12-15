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
        services.AddProviders(providerConfig);
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
            ProviderName = "Test Provider",
            PackageName = "Test Package",
            ChartColor = "#333333",
            Price = 9.99m,
            Currency = "USD",
            CreatedDate = DateTime.UtcNow,
            CreateById = Guid.NewGuid()
        };
        dbContext.PaymentHistories.Add(paymentHistory);
        await dbContext.SaveChangesAsync();

        var paymentHistoriesEndpoint = serviceProvider!.GetRequiredService<Provider.PaymentHistories.IRefitInterface>();
        var result = await paymentHistoriesEndpoint.GetAsync(new()
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

        var PaymentHistoriesEndpoint = serviceProvider!.GetRequiredService<Provider.PaymentHistories.IRefitInterface>();

        var payload = new Provider.PaymentHistories.Post.Payload
        {
            UserId = Guid.NewGuid(),
            ProviderName = "Test Provider",
            PackageName = "Basic Package",
            DiscountId = null,
            ChartColor = "#FFFFFF",
            Price = 9.99m,
            DiscountedPrice = null,
            Currency = "USD"
        };

        await PaymentHistoriesEndpoint.PostAsync(payload);

        var expected = await dbContext.PaymentHistories
            .FirstOrDefaultAsync(p => p.UserId == payload.UserId
                                   && p.ProviderName == payload.ProviderName
                                   && p.PackageName == payload.PackageName);

        Assert.NotNull(expected);
        Assert.Equal(payload.UserId, expected.UserId);
        Assert.Equal(payload.ProviderName, expected.ProviderName);
        Assert.Equal(payload.PackageName, expected.PackageName);
        Assert.Equal("USD", expected.Currency);
        Assert.Equal(9.99m, expected.Price);

        // Cleanup
        dbContext.PaymentHistories.Remove(expected);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task PUT_PaymentHistory()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();

        var paymentHistory = new Iowa.Databases.App.Tables.PaymentHistory.Table
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ProviderName = "Original Provider",
            PackageName = "Original Package",
            DiscountId = null,
            ChartColor = "#FFFFFF",
            Price = 9.99m,
            DiscountedPrice = null,
            Currency = "USD",
            CreatedDate = DateTime.UtcNow,
            CreateById = Guid.NewGuid()
        };

        dbContext.PaymentHistories.Add(paymentHistory);
        await dbContext.SaveChangesAsync();

        var endpoint = serviceProvider!.GetRequiredService<Provider.PaymentHistories.IRefitInterface>();
        var payload = new Provider.PaymentHistories.Put.Payload
        {
            Id = paymentHistory.Id,
            UserId = paymentHistory.UserId,
            ProviderName = "Updated Provider",
            PackageName = "Updated Package",
            DiscountId = null,
            ChartColor = "#FFFF00",
            Price = 10.99m,
            DiscountedPrice = null,
            Currency = "VND"
        };

        await endpoint.PutAsync(payload);

        await dbContext.Entry(paymentHistory).ReloadAsync();

        Assert.NotNull(paymentHistory);
        Assert.Equal("Updated Provider", paymentHistory.ProviderName);
        Assert.Equal("Updated Package", paymentHistory.PackageName);
        Assert.Equal("#FFFF00", paymentHistory.ChartColor);
        Assert.Equal("VND", paymentHistory.Currency);
        Assert.Equal(10.99m, paymentHistory.Price);

        // Cleanup
        dbContext.PaymentHistories.Remove(paymentHistory);
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
            ProviderName = "Test Provider",
            PackageName = "Test Package",
            DiscountId = null,
            ChartColor = "#000000",
            Price = 9.99m,
            DiscountedPrice = null,
            Currency = "USD",
            CreatedDate = DateTime.UtcNow,
            CreateById = Guid.NewGuid()
        };

        dbContext.PaymentHistories.Add(paymentHistory);
        await dbContext.SaveChangesAsync();

        var endpoint = serviceProvider!.GetRequiredService<Provider.PaymentHistories.IRefitInterface>();
        await endpoint.DeleteAsync(new Provider.PaymentHistories.Delete.Parameters { Id = paymentHistory.Id });
        await dbContext.Entry(paymentHistory).ReloadAsync();

        var deleted = await dbContext.PaymentHistories.FindAsync(paymentHistory.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task PATCH_PaymentHistory()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();

        var paymentHistory = new Iowa.Databases.App.Tables.PaymentHistory.Table
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ProviderName = "Original Provider",
            PackageName = "Original Package",
            DiscountId = null,
            ChartColor = "#FFFFFF",
            Price = 9.99m,
            DiscountedPrice = null,
            Currency = "USD",
            CreatedDate = DateTime.UtcNow,
            CreateById = Guid.NewGuid()
        };

        dbContext.PaymentHistories.Add(paymentHistory);
        await dbContext.SaveChangesAsync();

        var endpoint = serviceProvider!.GetRequiredService<Provider.PaymentHistories.IRefitInterface>();
        var operations = new List<Operation>
        {
            new Operation { op = "replace", path = "/ChartColor", value = "#FFFF00" },
            new Operation { op = "replace", path = "/Currency", value = "VND" },
            new Operation { op = "replace", path = "/Price", value = 49.99m },
            new Operation { op = "replace", path = "/ProviderName", value = "Patched Provider" },
            new Operation { op = "replace", path = "/PackageName", value = "Patched Package" }
        };

        await endpoint.PatchAsync(
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
        Assert.Equal("Patched Provider", paymentHistory.ProviderName);
        Assert.Equal("Patched Package", paymentHistory.PackageName);

        // Cleanup
        dbContext.PaymentHistories.Remove(paymentHistory);
        await dbContext.SaveChangesAsync();
    }

    #endregion
}