using LIMS.DDD.Service.Application.StudyTemplates.CalculationRules;
using LIMS.DDD.Service.Application.StudyTemplates.Core;
using LIMS.DDD.Service.Application.StudyTemplates.InputParameters;
using LIMS.DDD.Service.Application.StudyTemplates.ResultDefinitions;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS.DDD.Service.Application;

public static class DependencyInjection
{
    public static void AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<StudyTemplateCommandHandler>();
        services.AddScoped<StudyTemplateQueries>();

        services.AddScoped<InputParameterCommandHandler>();
        services.AddScoped<InputParameterQueries>();

        services.AddScoped<ResultDefinitionCommands>();
        services.AddScoped<ResultDefinitionQueries>();

        services.AddScoped<CalculationRuleCommandHandler>();
        services.AddScoped<CalculationRuleQueries>();
    }
}
