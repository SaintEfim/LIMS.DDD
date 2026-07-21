using LIMS.DDD.Service.API.Apis;
using LIMS.DDD.Service.Application;
using LIMS.DDD.Service.Application.StudyTemplates.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.Queries;
using LIMS.DDD.Service.Persistence;

namespace LIMS.DDD.Service.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddApplication();

        services.AddScoped<StudyTemplateServices>();

        return services;
    }
}
