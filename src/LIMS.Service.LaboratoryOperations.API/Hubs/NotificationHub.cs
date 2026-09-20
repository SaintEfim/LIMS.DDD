using Microsoft.AspNetCore.SignalR;

namespace LIMS.Service.LaboratoryOperations.API.Hubs;

public class NotificationHub : Hub
{
    public async Task Send(
        string message)
    {
        await Clients.All.SendAsync("Receive", message);
    }
}
