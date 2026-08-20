using LIMS.Service.Methodologies.API.Apis.CalculationRules;
using LIMS.Service.Methodologies.API.Apis.InputParameters;
using LIMS.Service.Methodologies.API.Apis.ResultDefinitions;
using LIMS.Service.Methodologies.API.Apis.StudyTemplates;
using LIMS.Service.Methodologies.Application;
using LIMS.Service.Methodologies.Domain.StudyTemplateAggregate.Services;
using LIMS.Service.Methodologies.Infrastructure;
using LIMS.Service.Methodologies.Persistence;

namespace LIMS.Service.Methodologies.API;

public static class DependencyInjection
{
    public static void AddApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructure();
        services.AddPersistence(configuration);
        services.AddApplication();

        services.AddScoped<StudyTemplateVersioningService>();

        services.AddScoped<InputParameterServices>();
        services.AddScoped<ResultDefinitionServices>();
        services.AddScoped<StudyTemplateServices>();
        services.AddScoped<CalculationRuleServices>();
    }
}
