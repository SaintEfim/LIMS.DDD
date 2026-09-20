using Microsoft.AspNetCore.SignalR;

namespace LIMS.Service.LaboratoryOperations.API.Hubs;

public class NotificationHubDto
{
    public Guid UserId { get; set; }

    public string NameOperation { get; set; } = string.Empty;

    public string Result { get; set; } = string.Empty;
}

public class NotificationHub : Hub
{
    // Надо писать в определённый hub
    public async Task Send(
        NotificationHubDto messageModel)
    {
        await Clients.All.SendAsync("Receive", messageModel);
    }
}
