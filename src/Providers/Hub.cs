using Iowa.Databases.App;
using Microsoft.AspNetCore.SignalR;

namespace Iowa.Providers;

public sealed class Hub : Microsoft.AspNetCore.SignalR.Hub
{
    #region [ Fields ]

    private readonly IowaContext _context;
    #endregion

    #region [ CTors ]

    public Hub(IowaContext context)
    {
        _context = context;
    }
    #endregion

    #region [Methods]
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId}: {message}");
    }
    #endregion
}

