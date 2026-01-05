using ExcelDataReader;
using System.Data;

namespace Iowa.Databases.App;

public class SeedFactory
{
    public async Task SeedProvider(IowaContext context)
    {
        if (context.Providers.Any())
        {
            Console.WriteLine("✓ Providers already seeded. Skipping...");
            return;
        }

        //// Lấy đường dẫn từ thư mục gốc của solution
        //var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //var projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\..\.."));
        //var filePath = Path.Combine(projectRoot, "iowa", "src", "Databases", "App", "Tables", "Provider", "Providers.xlsx");

        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases/App/Tables/Provider/Providers.xlsx");
        Console.WriteLine($"Looking for file at: {filePath}");
        Console.WriteLine($"File exists: {File.Exists(filePath)}");

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"⚠ File not found: {filePath}");
            return;
        }

        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var config = new ExcelDataSetConfiguration
        {
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            {
                UseHeaderRow = true
            }
        };

        var result = reader.AsDataSet(config);
        var table = result.Tables[0];

        var providers = table.AsEnumerable()
            .Where(x => !string.IsNullOrWhiteSpace(x.Field<string>("Id")) &&
                        !string.IsNullOrWhiteSpace(x.Field<string>("Name")) &&
                        !string.IsNullOrWhiteSpace(x.Field<string>("Description")))
            .Select(row =>
            {
                // Parse dates
                DateTime createdDate = DateTime.UtcNow;
                if (DateTime.TryParse(row["CreatedDate"]?.ToString(), out var parsedCreatedDate))
                {
                    createdDate = parsedCreatedDate;
                }

                DateTime? lastUpdated = null;
                var lastUpdatedStr = row["LastUpdated"]?.ToString();
                if (!string.IsNullOrWhiteSpace(lastUpdatedStr) && lastUpdatedStr.ToLower() != "null")
                {
                    if (DateTime.TryParse(lastUpdatedStr, out var parsedLastUpdated))
                    {
                        lastUpdated = parsedLastUpdated;
                    }
                }

                // Parse GUIDs
                Guid createdById = Guid.Empty;
                if (Guid.TryParse(row["CreatedById"]?.ToString(), out var parsedCreatedById))
                {
                    createdById = parsedCreatedById;
                }

                Guid? updatedById = null;
                var updatedByIdStr = row["UpdatedById"]?.ToString();
                if (!string.IsNullOrWhiteSpace(updatedByIdStr) && updatedByIdStr.ToLower() != "null")
                {
                    if (Guid.TryParse(updatedByIdStr, out var parsedUpdatedById))
                    {
                        updatedById = parsedUpdatedById;
                    }
                }

                return new Iowa.Databases.App.Tables.Provider.Table
                {
                    Id = Guid.Parse(row["Id"].ToString()!),
                    Name = row["Name"].ToString()!,
                    Description = row["Description"].ToString()!,
                    IconUrl = row["IconUrl"]?.ToString() ?? string.Empty,
                    WebsiteUrl = row["WebsiteUrl"]?.ToString() ?? string.Empty,
                    CreatedDate = createdDate,
                    LastUpdated = lastUpdated,
                    CreatedById = createdById,
                    UpdatedById = updatedById
                };
            }).ToList();

        context.Providers.AddRange(providers);
        await context.SaveChangesAsync();

        Console.WriteLine($"✓ Seeded {providers.Count} providers successfully.");
    }

    public async Task SeedPackage(IowaContext context)
    {
        if (context.Packages.Any())
        {
            Console.WriteLine("✓ Packages already seeded. Skipping...");
            return;
        }

        // Lấy đường dẫn từ thư mục gốc của solution
        //var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        //var projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, @"..\..\..\..\.."));
        //var filePath = Path.Combine(projectRoot, "iowa", "src", "Databases", "App", "Tables", "Package", "Packages.xlsx");
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Databases/App/Tables/Package/Packages.xlsx");

        Console.WriteLine($"Looking for file at: {filePath}");
        Console.WriteLine($"File exists: {File.Exists(filePath)}");

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"⚠ File not found: {filePath}");
            return;
        }

        using var stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
        using var reader = ExcelReaderFactory.CreateReader(stream);

        var config = new ExcelDataSetConfiguration
        {
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
            {
                UseHeaderRow = true
            }
        };

        var result = reader.AsDataSet(config);
        var table = result.Tables[0];

        var packages = table.AsEnumerable()
            .Where(x => !string.IsNullOrWhiteSpace(x.Field<string>("Id")) &&
                        !string.IsNullOrWhiteSpace(x.Field<string>("ProviderId")) &&
                        !string.IsNullOrWhiteSpace(x.Field<string>("Name")) &&
                        !string.IsNullOrWhiteSpace(x.Field<string>("Description")))
            .Select(row =>
            {
                // Parse dates
                DateTime createdDate = DateTime.UtcNow;
                if (DateTime.TryParse(row["CreatedDate"]?.ToString(), out var parsedCreatedDate))
                {
                    createdDate = parsedCreatedDate;
                }

                DateTime? lastUpdated = null;
                var lastUpdatedStr = row["LastUpdated"]?.ToString();
                if (!string.IsNullOrWhiteSpace(lastUpdatedStr) && lastUpdatedStr.ToLower() != "null")
                {
                    if (DateTime.TryParse(lastUpdatedStr, out var parsedLastUpdated))
                    {
                        lastUpdated = parsedLastUpdated;
                    }
                }

                // Parse GUIDs
                Guid createdById = Guid.Empty;
                if (Guid.TryParse(row["CreatedById"]?.ToString(), out var parsedCreatedById))
                {
                    createdById = parsedCreatedById;
                }

                Guid? updatedById = null;
                var updatedByIdStr = row["UpdatedById"]?.ToString();
                if (!string.IsNullOrWhiteSpace(updatedByIdStr) && updatedByIdStr.ToLower() != "null")
                {
                    if (Guid.TryParse(updatedByIdStr, out var parsedUpdatedById))
                    {
                        updatedById = parsedUpdatedById;
                    }
                }

                // Parse Price
                decimal? price = null;
                var priceStr = row["Price"]?.ToString();
                if (!string.IsNullOrWhiteSpace(priceStr) && priceStr.ToLower() != "null")
                {
                    // Remove any comma separators (e.g., "483,48" -> "483.48")
                    priceStr = priceStr.Replace(",", ".");
                    if (decimal.TryParse(priceStr, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var parsedPrice))
                    {
                        price = parsedPrice;
                    }
                }

                return new Iowa.Databases.App.Tables.Package.Table
                {
                    Id = Guid.Parse(row["Id"].ToString()!),
                    ProviderId = Guid.Parse(row["ProviderId"].ToString()!),
                    Name = row["Name"].ToString()!,
                    Description = row["Description"].ToString()!,
                    IconUrl = row["IconUrl"]?.ToString() ?? string.Empty,
                    Price = price,
                    Currency = row["Currency"]?.ToString() ?? string.Empty,
                    CreatedDate = createdDate,
                    LastUpdated = lastUpdated,
                    CreatedById = createdById,
                    UpdatedById = updatedById
                };
            }).ToList();

        context.Packages.AddRange(packages);
        await context.SaveChangesAsync();

        Console.WriteLine($"✓ Seeded {packages.Count} packages successfully.");
    }
}
