using Guides.Service.API.Apis;
using Guides.Service.Application;
using Guides.Service.Infrastructure;
using Guides.Service.Persistence;

namespace Guides.Service.API;

public static class DependencyInjection
{
    public static void AddApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddInfrastructure();
        services.AddApplication();

        services.AddScoped<UnitServices>();
    }
}
