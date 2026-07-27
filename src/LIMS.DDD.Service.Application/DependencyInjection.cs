using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules.Queries;
using LIMS.DDD.Service.Application.StudyTemplates.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.Queries;
using LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions.Queries;
using LIMS.DDD.Service.Application.StudyTemplates.InputParameters.Commands;
using LIMS.DDD.Service.Application.StudyTemplates.InputParameters.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS.DDD.Service.Application;

public static class DependencyInjection
{
    public static void AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<StudyTemplateCommands>();
        services.AddScoped<StudyTemplateQueries>();

        services.AddScoped<InputParameterCommands>();
        services.AddScoped<InputParameterQueries>();

        services.AddScoped<ResultDefinitionCommands>();
        services.AddScoped<ResultDefinitionQueries>();

        services.AddScoped<CalculationRuleCommands>();
        services.AddScoped<CalculationRuleQueries>();
    }
}
