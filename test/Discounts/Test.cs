using Iowa.Databases.App;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Provider;
using Provider.Discounts.Patch;

namespace Test.Discounts;
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
        var package = new Iowa.Databases.App.Tables.Discount.Table
        {
            Id = Guid.NewGuid(),
            ProviderId = Guid.NewGuid(),
            Code = "TEST DISCOUNT",
            Description = "A basic TEST DISCOUNT for testing purposes.",
            DiscountType = "Percentage",
            DiscountValue = 10.0m,
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };
        dbContext.Discounts.Add(package);
        await dbContext.SaveChangesAsync();
        var exercisesEndpoint = serviceProvider!.GetRequiredService<Provider.Discounts.IRefitInterface>();
        var result = await exercisesEndpoint.Get(new()
        {
        });
        var items = result?.Content?.Items;

        Assert.NotNull(result);
        Assert.NotNull(result);
        Assert.NotNull(result.Content);
        await dbContext.SaveChangesAsync();
    }
    [Fact]
    public async Task POST_Discount()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();
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
        dbContext.Providers.Add(provider);
        await dbContext.SaveChangesAsync();

        Guid id = Guid.NewGuid();
        string code = $"TEST DISCOUNT {id}";
        string description = "A basic TEST DISCOUNT for testing purposes."; 
        var discountsEndpoint = serviceProvider!.GetRequiredService<Provider.Discounts.IRefitInterface>();
        var payload = new Provider.Discounts.Post.Payload
        {
            ProviderId = provider.Id,
            Code = code,
            Description = description,
            DiscountType = "Percentage",
            DiscountValue = 10.0m
        };
        await discountsEndpoint.Post(payload);

        var createdDiscount = await dbContext.Discounts.FirstOrDefaultAsync(d => d.Code == code && d.ProviderId == provider.Id);
        Assert.NotNull(createdDiscount);
        Assert.Equal(description, createdDiscount!.Description);
        Assert.Equal(10.0m, createdDiscount.DiscountValue);
        Assert.Equal("Percentage", createdDiscount.DiscountType);
        Assert.Equal(code, createdDiscount.Code);
        
        // Clean up
        dbContext.Discounts.Remove(createdDiscount!);
        dbContext.Providers.Remove(provider);
        await dbContext.SaveChangesAsync();
    }
    [Fact]
    public async Task PUT_Discount()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();
        var provider = new Iowa.Databases.App.Tables.Provider.Table
        {
            Id = Guid.NewGuid(),
            Name = "TEST PROVIDER",
            Description = "Provider for PUT test",
            IconUrl = "https://example.com/provider.png",
            WebsiteUrl = "https://example.com",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            CreatedById = Guid.NewGuid()
        };
        dbContext.Providers.Add(provider);
        await dbContext.SaveChangesAsync();

        var id = Guid.NewGuid();
        var existingDiscount = new Iowa.Databases.App.Tables.Discount.Table
        {
            Id = id,
            ProviderId = provider.Id,
            Code = "TEST DISCOUNT PUT",
            Description = "A basic TEST DISCOUNT for testing purposes.",
            DiscountType = "Percentage",
            DiscountValue = 10.0m,
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };
        dbContext.Discounts.Add(existingDiscount);
        await dbContext.SaveChangesAsync();

        var discountsEndpoint = serviceProvider!.GetRequiredService<Provider.Discounts.IRefitInterface>();
        var payload = new Provider.Discounts.Put.Payload
        {
            Id = id,
            ProviderId = provider.Id,
            Code = "UPDATED TEST DISCOUNT PUT",
            Description = "An updated TEST DISCOUNT for testing purposes.",
            DiscountType = "FixedAmount",
            DiscountValue = 25.0m
        };
        await discountsEndpoint.Put(payload);

        await dbContext.Entry(existingDiscount).ReloadAsync();
        var updatedDiscount = existingDiscount;
        Assert.NotNull(updatedDiscount);
        Assert.Equal("UPDATED TEST DISCOUNT PUT", updatedDiscount.Code);
        Assert.Equal("An updated TEST DISCOUNT for testing purposes.", updatedDiscount.Description);
        Assert.Equal("FixedAmount", updatedDiscount.DiscountType);
        Assert.Equal(25.0m, updatedDiscount.DiscountValue);
        
        // Clean up
        dbContext.Discounts.Remove(updatedDiscount);
        dbContext.Providers.Remove(provider);
        await dbContext.SaveChangesAsync();
    }
    [Fact]
    public async Task PATCH_UpdateDiscount()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();
        var provider = new Iowa.Databases.App.Tables.Provider.Table
        {
            Id = Guid.NewGuid(),
            Name = "TEST PROVIDER",
            Description = "Provider for PATCH test",
            IconUrl = "https://example.com/provider.png",
            WebsiteUrl = "https://example.com",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            CreatedById = Guid.NewGuid()
        };
        dbContext.Providers.Add(provider);
        await dbContext.SaveChangesAsync();
        var id = Guid.NewGuid();
        var existingDiscount = new Iowa.Databases.App.Tables.Discount.Table
        {
            Id = id,
            ProviderId = provider.Id,
            Code = "TEST DISCOUNT PATCH",
            Description = "A basic TEST DISCOUNT for testing purposes.",
            DiscountType = "Percentage",
            DiscountValue = 10.0m,
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };
        dbContext.Discounts.Add(existingDiscount);
        await dbContext.SaveChangesAsync();
        var discountsEndpoint = serviceProvider!.GetRequiredService<Provider.Discounts.IRefitInterface>();
        var operations = new List<Operation>
        {
            new Operation
            {
                op = "replace",
                path = "/Description",
                value = "Updated description for PATCH test"
            },
            new Operation
            {
                op = "replace",
                path = "/DiscountValue",
                value = 15.0m
            },
            new Operation
            {
                op = "replace",
                path = "/Code",
                value = "UPDATED CODE"
            }
        };
        await discountsEndpoint.Patch(new Provider.Discounts.Patch.Parameters {Id = id }, operations);
        await dbContext.Entry(existingDiscount).ReloadAsync();
        var patchedDiscount = existingDiscount;
        Assert.Equal("Updated description for PATCH test", patchedDiscount.Description);
        Assert.Equal(15.0m, patchedDiscount.DiscountValue);
        Assert.Equal("UPDATED CODE", patchedDiscount.Code);
        dbContext.Discounts.Remove(patchedDiscount);
        dbContext.Providers.Remove(provider);
        await dbContext.SaveChangesAsync();
    }
    [Fact]
    public async Task DELETE_RemoveDiscount()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();

        var discount = new Iowa.Databases.App.Tables.Discount.Table
        {
            Id = Guid.NewGuid(),
            ProviderId = Guid.NewGuid(),
            Code = "TEST DISCOUNT DELETE",
            Description = "A basic TEST DISCOUNT for testing purposes.",
            DiscountType = "Percentage",
            DiscountValue = 10.0m,
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };
        dbContext.Discounts.Add(discount);
        await dbContext.SaveChangesAsync();

        var id = Guid.NewGuid();
        var existingDiscount = new Iowa.Databases.App.Tables.Discount.Table
        {
            Id = id,
            ProviderId = Guid.NewGuid(),
            Code = "TEST DISCOUNT TO DELETE",
            Description = "A basic TEST DISCOUNT for deletion test.",
            DiscountType = "Percentage",
            DiscountValue = 20.0m,
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };
        dbContext.Discounts.Add(existingDiscount);
        await dbContext.SaveChangesAsync();
        var discountsEndpoint = serviceProvider!.GetRequiredService<Provider.Discounts.IRefitInterface>();  
        await discountsEndpoint.Delete(new Provider.Discounts.Delete.Parameters { Id = id });

        await dbContext.Entry(existingDiscount).ReloadAsync();

        var deletedDiscount = await dbContext.Discounts.FindAsync(id);
        Assert.Null(deletedDiscount);

        dbContext.Discounts.Remove(discount);
        await dbContext.SaveChangesAsync();
    }
    #endregion
}
    
