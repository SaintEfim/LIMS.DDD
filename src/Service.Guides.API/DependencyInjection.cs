using Service.Guides.Application;
using Service.Guides.Infrastructure;
using Service.Guides.Persistence;
using Service.Guides.API.Apis;

namespace Service.Guides.API;

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
