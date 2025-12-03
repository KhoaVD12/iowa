using Iowa.Databases.App;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Provider;

namespace Test.Packages;

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
        var exercisesEndpoint = serviceProvider!.GetRequiredService<Provider.Packages.IRefitInterface>();
        var result = await exercisesEndpoint.Get(new()
        {
        });
        var items = result?.Content?.Items;

        Assert.NotNull(result);
        Assert.NotNull(result);
        Assert.NotNull(result.Content);
        Assert.True(items.Count > 0, "Expected at least one exercise in result.");
        Assert.Contains(items, p => p.Id == package.Id);
        dbContext.Packages.Remove(package);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task POST_Package()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();

        // Tạo provider trước
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
        string name = $"TEST PACKAGE {id}";
        string description = "A basic TEST PACKAGE for testing POST.";
        var packagesEndpoint = serviceProvider!.GetRequiredService<Provider.Packages.IRefitInterface>();

        var payload = new Provider.Packages.Post.Payload
        {
            ProviderId = provider.Id,
            Name = name,
            Description = description,
            IconUrl = "https://example.com/icon.png",
            Price = 19.99m,
            Currency = "USD"
        };

        await packagesEndpoint.Post(payload);

        var expected = await dbContext.Packages.FirstOrDefaultAsync(p => p.Name == name);
        Assert.NotNull(expected);
        Assert.Equal(name, expected.Name);
        Assert.Equal(description, expected.Description);
        Assert.Equal("USD", expected.Currency);

        // Cleanup
        dbContext.Packages.Remove(expected);
        dbContext.Providers.Remove(provider);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task PUT_Package()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();

        // Tạo provider trước
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
        var existingPackage = new Iowa.Databases.App.Tables.Package.Table
        {
            Id = id,
            ProviderId = provider.Id,
            Name = "TEST PACKAGE",
            Description = "Original description",
            IconUrl = "https://example.com/icon.png",
            Price = 9.99m,
            Currency = "USD",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };
        dbContext.Packages.Add(existingPackage);
        await dbContext.SaveChangesAsync();

        var packagesEndpoint = serviceProvider!.GetRequiredService<Provider.Packages.IRefitInterface>();
        var payload = new Provider.Packages.Put.Payload
        {
            Id = id,
            Name = "UPDATED TEST PACKAGE",
            ProviderId = provider.Id,
            Description = "Updated description for PUT test",
            IconUrl = "https://example.com/icon2.png",
            Price = 29.99m,
            Currency = "USD"
        };

        await packagesEndpoint.Put(payload);

        await dbContext.Entry(existingPackage).ReloadAsync();
        var updatedPackage = existingPackage;

        Assert.NotNull(updatedPackage);
        Assert.Equal("UPDATED TEST PACKAGE", updatedPackage.Name);
        Assert.Equal("Updated description for PUT test", updatedPackage.Description);
        Assert.Equal(29.99m, updatedPackage.Price);

        // Cleanup
        dbContext.Packages.Remove(updatedPackage);
        dbContext.Providers.Remove(provider);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task DELETE_Package()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();

        // Tạo provider trước
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
        dbContext.Providers.Add(provider);
        await dbContext.SaveChangesAsync();

        var id = Guid.NewGuid();
        var existingPackage = new Iowa.Databases.App.Tables.Package.Table
        {
            Id = id,
            ProviderId = provider.Id,
            Name = "TEST PACKAGE DELETE",
            Description = "Package created for DELETE test",
            IconUrl = "https://example.com/icon.png",
            Price = 9.99m,
            Currency = "USD",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };
        dbContext.Packages.Add(existingPackage);
        await dbContext.SaveChangesAsync();

        var packagesEndpoint = serviceProvider!.GetRequiredService<Provider.Packages.IRefitInterface>();
        await packagesEndpoint.Delete(new Provider.Packages.Delete.Parameters { Id = id });

        await dbContext.Entry(existingPackage).ReloadAsync();

        var deletedPackage = await dbContext.Packages.FindAsync(id);
        Assert.Null(deletedPackage);

        // Cleanup provider
        dbContext.Providers.Remove(provider);
        await dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task PATCH_Package()
    {
        var dbContext = serviceProvider!.GetRequiredService<IowaContext>();

        // Tạo provider trước
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
        var existingPackage = new Iowa.Databases.App.Tables.Package.Table
        {
            Id = id,
            ProviderId = provider.Id,
            Name = "PATCH TEST PACKAGE",
            Description = "Original description for PATCH test",
            IconUrl = "https://example.com/icon.png",
            Price = 9.99m,
            Currency = "USD",
            CreatedDate = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };
        dbContext.Packages.Add(existingPackage);
        await dbContext.SaveChangesAsync();

        var packagesEndpoint = serviceProvider!.GetRequiredService<Provider.Packages.IRefitInterface>();
        var operations = new List<Provider.Packages.Patch.Operation>
    {
        new Provider.Packages.Patch.Operation { op = "replace", path = "/Name", value = "PATCHED PACKAGE NAME" },
        new Provider.Packages.Patch.Operation { op = "replace", path = "/Description", value = "Patched description" },
        new Provider.Packages.Patch.Operation { op = "replace", path = "/Price", value = 49.99m }
    };

        await packagesEndpoint.Patch(new Provider.Packages.Patch.Parameters { Id = id }, operations);

        await dbContext.Entry(existingPackage).ReloadAsync();
        var patchedPackage = existingPackage;

        Assert.NotNull(patchedPackage);
        Assert.Equal("PATCHED PACKAGE NAME", patchedPackage.Name);
        Assert.Equal("Patched description", patchedPackage.Description);
        Assert.Equal(49.99m, patchedPackage.Price);

        // Cleanup
        dbContext.Packages.Remove(patchedPackage);
        dbContext.Providers.Remove(provider);
        await dbContext.SaveChangesAsync();
    }

    #endregion
}
