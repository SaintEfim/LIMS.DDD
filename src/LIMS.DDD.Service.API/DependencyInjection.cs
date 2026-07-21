using LIMS.DDD.Service.API.Apis.StudyTemplateParameters;
using LIMS.DDD.Service.API.Apis.StudyTemplateResults;
using LIMS.DDD.Service.API.Apis.StudyTemplates;
using LIMS.DDD.Service.Application;
using LIMS.DDD.Service.Persistence;

namespace LIMS.DDD.Service.API;

public static class DependencyInjection
{
    public static void AddApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddApplication();

        services.AddScoped<StudyTemplateParameterServices>();
        services.AddScoped<StudyTemplateResultServices>();
        services.AddScoped<StudyTemplateServices>();
    }
}
