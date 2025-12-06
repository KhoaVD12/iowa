using Iowa.Databases.App;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Provider;
using Provider.Providers.Operations.Patch;

namespace Test.Providers;

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
        var provider = new Iowa.Databases.App.Tables.Provider.Table
        {
            Id = Guid.NewGuid(),
            Name = "TEST PROVIDER",
            Description = "A basic TEST PROVIDER for testing purposes.",
            IconUrl = "https://example.com/provider.png",
            WebsiteUrl = "https://example.com",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };
        dbContext.Providers.Add(provider);
        await dbContext.SaveChangesAsync();
        var exercisesEndpoint = serviceProvider!.GetRequiredService<Provider.Providers.IRefitInterface>();
        var result = await exercisesEndpoint.Get(new()
        {
        });
        var items = result?.Content?.Items;

        Assert.NotNull(result);
        Assert.NotNull(result);
        Assert.NotNull(result.Content);
        Assert.True(items.Count > 0, "Expected at least one exercise in result.");
        Assert.Contains(items, p => p.Id == provider.Id);
        dbContext.Providers.Remove(provider);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task POST_Package()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();

        var endpoint = serviceProvider!.GetRequiredService<Provider.Providers.IRefitInterface>();

        var id = Guid.NewGuid();
        var name = $"TEST PROVIDER {id}";

        var payload = new Provider.Providers.Operations.Post.Payload
        {
            Name = name,
            Description = "POST TEST PROVIDER",
            IconUrl = "https://example.com/test.png",
            WebsiteUrl = "https://example.com"
        };

        await endpoint.Post(payload);

        var expected = await dbContext.Providers.FirstOrDefaultAsync(p => p.Name == name);

        Assert.NotNull(expected);
        Assert.Equal(name, expected.Name);
        Assert.Equal("POST TEST PROVIDER", expected.Description);

        // Cleanup
        dbContext.Providers.Remove(expected);
        await dbContext.SaveChangesAsync();
    }
    [Fact]
    public async Task PUT_Provider()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();

        var provider = new Iowa.Databases.App.Tables.Provider.Table
        {
            Id = Guid.NewGuid(),
            Name = "ORIGINAL PROVIDER",
            Description = "Original description",
            IconUrl = "https://example.com/original.png",
            WebsiteUrl = "https://example.com",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        dbContext.Providers.Add(provider);
        await dbContext.SaveChangesAsync();

        var endpoint = serviceProvider!.GetRequiredService<Provider.Providers.IRefitInterface>();

        var payload = new Provider.Providers.Operations.Put.Payload
        {
            Id = provider.Id,
            Name = "UPDATED PROVIDER",
            Description = "Updated description",
            IconUrl = "https://example.com/updated.png",
            WebsiteUrl = "https://updated.com"
        };

        await endpoint.Put(payload);

        await dbContext.Entry(provider).ReloadAsync();

        Assert.Equal("UPDATED PROVIDER", provider.Name);
        Assert.Equal("Updated description", provider.Description);
        Assert.Equal("https://updated.com", provider.WebsiteUrl);

        dbContext.Providers.Remove(provider);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task DELETE_Provider()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();

        var provider = new Iowa.Databases.App.Tables.Provider.Table
        {
            Id = Guid.NewGuid(),
            Name = "DELETE TEST PROVIDER",
            Description = "For DELETE test",
            IconUrl = "https://example.com/test.png",
            WebsiteUrl = "https://example.com",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        dbContext.Providers.Add(provider);
        await dbContext.SaveChangesAsync();

        var endpoint = serviceProvider!.GetRequiredService<Provider.Providers.IRefitInterface>();

        await endpoint.Delete(new Provider.Providers.Operations.Delete.Parameters { Id = provider.Id });

        var deleted = await dbContext.Providers.FindAsync(provider.Id);
        Assert.Null(deleted);
    }


    [Fact]
    public async Task PATCH_Provider()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();

        var provider = new Iowa.Databases.App.Tables.Provider.Table
        {
            Id = Guid.NewGuid(),
            Name = "PATCH PROVIDER",
            Description = "Original description",
            IconUrl = "https://example.com/icon.png",
            WebsiteUrl = "https://example.com",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        dbContext.Providers.Add(provider);
        await dbContext.SaveChangesAsync();

        var endpoint = serviceProvider!.GetRequiredService<Provider.Providers.IRefitInterface>();

        var operations = new List<Operation>
        {
            new Operation { op = "replace", path = "/Name", value = "PATCHED PROVIDER" },
            new Operation { op = "replace", path = "/Description", value = "Patched description" },
            new Operation { op = "replace", path = "/WebsiteUrl", value = "https://patched.com" }
        };

        await endpoint.Patch(
            new Provider.Providers.Operations.Patch.Parameters { Id = provider.Id },
            operations
        );

        await dbContext.Entry(provider).ReloadAsync();

        Assert.Equal("PATCHED PROVIDER", provider.Name);
        Assert.Equal("Patched description", provider.Description);
        Assert.Equal("https://patched.com", provider.WebsiteUrl);

        dbContext.Providers.Remove(provider);
        await dbContext.SaveChangesAsync();
    }
    #endregion

}
