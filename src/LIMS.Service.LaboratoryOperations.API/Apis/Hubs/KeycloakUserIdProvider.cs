using Microsoft.AspNetCore.SignalR;

namespace LIMS.Service.LaboratoryOperations.API.Apis.Hubs;

public sealed class KeycloakUserIdProvider : IUserIdProvider
{
    public string? GetUserId(
        HubConnectionContext connection) =>
        connection.User.FindFirst("sub")
            ?.Value;
}
