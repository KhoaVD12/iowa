using Iowa.Databases.App;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Provider;
using Provider.Discounts.Patch;
/*
namespace Test.Discounts
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
        Assert.True(items.Count > 0, "Expected at least one exercise in result.");
        Assert.Contains(items, p => p.Id == package.Id);
        dbContext.Discounts.Remove(discount);
        await dbContext.SaveChangesAsync();
    }
    


}*/