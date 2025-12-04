using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Iowa.Databases.Identity;

public class IdentityContext: IdentityDbContext
{
    public IdentityContext(DbContextOptions<IdentityContext> options): base(options)
    {
        
    }
}
