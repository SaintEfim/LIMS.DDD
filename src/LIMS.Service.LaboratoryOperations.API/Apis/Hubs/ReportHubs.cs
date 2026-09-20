using Carter;
using LIMS.Service.LaboratoryOperations.API.Hubs;

namespace LIMS.Service.LaboratoryOperations.API.Apis.Hubs;

public class ReportHubs : ICarterModule
{
    public void AddRoutes(
        IEndpointRouteBuilder app)
    {
        app.MapHub<NotificationHub>("/reports")
            .RequireAuthorization();
    }
}
