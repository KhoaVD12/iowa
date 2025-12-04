using Iowa.Databases.App;
using Microsoft.EntityFrameworkCore;

namespace Iowa.Subscriptions.Delete.Messager;

public class Handler
{
    private readonly IowaContext _context;
    public Handler(IowaContext context)
    {
        _context = context;
    }
    
}
