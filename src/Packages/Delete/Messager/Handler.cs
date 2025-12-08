using Iowa.Databases.App;
using Microsoft.EntityFrameworkCore;

namespace Iowa.Packages.Delete.Messager;

public class Handler(IowaContext context)
{
    private readonly IowaContext _context = context;

    public async Task Handle(Message message)
    {
        var existSubscriptions = _context.Subscriptions.Where(x => x.PackageId == message.Id);
        if (existSubscriptions.Any())
        {
            await _context.Subscriptions.Where(x => x.PackageId == message.Id).ExecuteDeleteAsync();
        }
        await _context.SaveChangesAsync();
    }
}
