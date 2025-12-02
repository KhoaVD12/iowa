using Iowa.Databases.App;
using Microsoft.EntityFrameworkCore;

namespace Iowa.Packages.Delete.Messager;

public class Handler
{
    private readonly IowaContext _context;
    public Handler(IowaContext context)
    {
        _context = context;
    }
    public async Task Handle(Message message)
    {
        var existSubscriptions = _context.Subcriptions.Where(x => x.PackageId == message.Id);
        if(existSubscriptions.Any())
        {
            await _context.Subcriptions.Where(x => x.PackageId == message.Id).ExecuteDeleteAsync();
        }
        await _context.SaveChangesAsync();
    }
}
