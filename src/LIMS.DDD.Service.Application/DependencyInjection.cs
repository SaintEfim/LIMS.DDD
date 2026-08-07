using LIMS.DDD.Service.Application.Orders;
using LIMS.DDD.Service.Application.Samples;
using LIMS.DDD.Service.Application.Studies.Core;
using LIMS.DDD.Service.Application.Studies.MeasuredValues;
using LIMS.DDD.Service.Application.Studies.TestResults;
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
        services.AddScoped<StudyTemplateCommandsHandler>();
        services.AddScoped<InputParameterCommandsHandler>();
        services.AddScoped<ResultDefinitionCommandsHandler>();
        services.AddScoped<CalculationRuleCommandsHandler>();

        services.AddScoped<StudyTemplateQueries>();
        services.AddScoped<InputParameterQueries>();
        services.AddScoped<ResultDefinitionQueries>();
        services.AddScoped<CalculationRuleQueries>();

        services.AddScoped<OrderCommandsHandler>();
        services.AddScoped<SampleCommandsHandler>();
        services.AddScoped<StudyCommandsHandler>();
        services.AddScoped<MeasuredValueCommandsHandler>();
        services.AddScoped<TestResultCommandsHandler>();

        services.AddScoped<OrderQueries>();
        services.AddScoped<SampleQueries>();
        services.AddScoped<StudyQueries>();
        services.AddScoped<MeasuredValueQueries>();
        services.AddScoped<TestResultQueries>();
    }
}
