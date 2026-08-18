using LIMS.Service.Methodologies.Application.StudyTemplates.CalculationRules;
using LIMS.Service.Methodologies.Application.StudyTemplates.Core;
using LIMS.Service.Methodologies.Application.StudyTemplates.InputParameters;
using LIMS.Service.Methodologies.Application.StudyTemplates.ResultDefinitions;
using LIMS.Service.Methodologies.Application.Units;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS.Service.Methodologies.Application;

public static class DependencyInjection
{
    public static void AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<UnitSnapshotCommandsHandler>();

        services.AddScoped<StudyTemplateCommandsHandler>();
        services.AddScoped<InputParameterCommandsHandler>();
        services.AddScoped<ResultDefinitionCommandsHandler>();
        services.AddScoped<CalculationRuleCommandsHandler>();

        services.AddScoped<StudyTemplateQueries>();
        services.AddScoped<InputParameterQueries>();
        services.AddScoped<ResultDefinitionQueries>();
        services.AddScoped<CalculationRuleQueries>();
    }
}
