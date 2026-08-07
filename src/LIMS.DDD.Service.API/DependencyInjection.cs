using LIMS.DDD.Service.API.Apis.CalculationRules;
using LIMS.DDD.Service.API.Apis.InputParameters;
using LIMS.DDD.Service.API.Apis.ResultDefinitions;
using LIMS.DDD.Service.API.Apis.StudyTemplates;
using LIMS.DDD.Service.Application;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.SampleAggregate.Services;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.StudyAggregate.Services;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Services;
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

        services.AddScoped<StudyTemplateVersioningService>();
        services.AddScoped<SampleCreationDomainService>();
        services.AddScoped<SampleDeletionDomainService>();
        services.AddScoped<StudyCreationDomainService>();

        services.AddScoped<InputParameterServices>();
        services.AddScoped<ResultDefinitionServices>();
        services.AddScoped<StudyTemplateServices>();
        services.AddScoped<CalculationRuleServices>();
    }
}
