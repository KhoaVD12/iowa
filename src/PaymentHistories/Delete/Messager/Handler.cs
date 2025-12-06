using Iowa.Databases.App;
using Microsoft.EntityFrameworkCore;

namespace Iowa.PaymentHistories.Delete.Messager;

public class Handler
{
    private readonly IowaContext _context;
    public Handler(IowaContext context)
    {
        _context = context;
    }
    
}
