using Carter;
using LIMS.Service.LaboratoryOperations.API.Hubs;

namespace LIMS.Service.LaboratoryOperations.API.Apis.Hubs;

public class HubModule : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        app.MapHub<NotificationHub>("/hubs/notifications")
            .RequireAuthorization();
    }
}
