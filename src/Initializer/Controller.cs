using Iowa.Databases.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Iowa.Initializer;

[ApiController]
[Authorize]
[Route("api/initializer")]
public class Controller(IowaContext iowaDbContext) : ControllerBase
{
    private readonly IowaContext iowaDbContext = iowaDbContext;
    private readonly Databases.App.SeedFactory iowaSeeder = new Databases.App.SeedFactory();

    [HttpPost("seed-providers")]
    public async Task<IActionResult> SeedProviders()
    {
        try
        {
            await iowaSeeder.SeedProvider(iowaDbContext);
            return Created(string.Empty, "Providers seeded successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to seed providers.", error = ex.Message });
        }
    }

    [HttpPost("seed-packages")]
    public async Task<IActionResult> SeedPackages()
    {
        try
        {
            await iowaSeeder.SeedPackage(iowaDbContext);
            return Created(string.Empty, "Packages seeded successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Failed to seed packages.", error = ex.Message });
        }
    }
}