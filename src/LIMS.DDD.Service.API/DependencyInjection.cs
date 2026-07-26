using LIMS.DDD.Service.API.Apis.CalculationRules;
using LIMS.DDD.Service.API.Apis.StudyTemplateDeterminations;
using LIMS.DDD.Service.API.Apis.StudyTemplateObservations;
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

        services.AddScoped<StudyTemplateObservationServices>();
        services.AddScoped<StudyTemplateDeterminationServices>();
        services.AddScoped<StudyTemplateServices>();
        services.AddScoped<CalculationRuleServices>();
    }
}
