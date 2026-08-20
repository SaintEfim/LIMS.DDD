using Application.SeedWork.SeedWork;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS.Service.Methodologies.Application;

public static class DependencyInjection
{
    public static void AddApplication(
        this IServiceCollection services)
    {
        services.AddQueries(typeof(DependencyInjection).Assembly);
        services.AddCommandsHandlers(typeof(DependencyInjection).Assembly);
    }
}
