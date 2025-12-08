using Iowa.Databases.App;
using Microsoft.EntityFrameworkCore;

namespace Iowa.Providers.Delete.Messager;

public class Handler
{
    private readonly IowaContext _context;
    public Handler(IowaContext context)
    {
        _context = context;
    }

    public async Task Handle(Message message)
    {
        var existPackages = _context.Packages.Where(x => x.ProviderId == message.Id);
        if (existPackages.Any())
        {
            await _context.Packages.Where(x => x.ProviderId == message.Id).ExecuteDeleteAsync();
        }
        await _context.SaveChangesAsync();


        var existSubcriptions = _context.Subscriptions.Where(x => x.ProviderId == message.Id);
        if (existSubcriptions.Any())
        {
            await _context.Subscriptions.Where(x => x.ProviderId == message.Id).ExecuteDeleteAsync();
        }
        await _context.SaveChangesAsync();

    }

}
