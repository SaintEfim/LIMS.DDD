using LIMS.DDD.Service.API.Apis.CalculationRules;
using LIMS.DDD.Service.API.Apis.InputParameters;
using LIMS.DDD.Service.API.Apis.MeasuredValues;
using LIMS.DDD.Service.API.Apis.Orders;
using LIMS.DDD.Service.API.Apis.ResultDefinitions;
using LIMS.DDD.Service.API.Apis.Samples;
using LIMS.DDD.Service.API.Apis.Studies;
using LIMS.DDD.Service.API.Apis.StudyTemplates;
using LIMS.DDD.Service.API.Apis.TestResults;
using LIMS.DDD.Service.Application;
using LIMS.DDD.Service.Domain.LaboratoryOperationsContext.Services;
using LIMS.DDD.Service.Domain.StudyTemplateContext.StudyTemplateAggregate.Services;
using LIMS.DDD.Service.Infrastructure;
using LIMS.DDD.Service.Persistence;

namespace LIMS.DDD.Service.API;

public static class DependencyInjection
{
    public static void AddApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructure();
        services.AddPersistence(configuration);
        services.AddApplication();

        services.AddScoped<StudyStatusChangeDomainService>();
        services.AddScoped<SampleStatusChangeDomainService>();
        services.AddScoped<StudyStatusChangeDomainService>();
        services.AddScoped<OrderStatusChangeDomainService>();

        services.AddScoped<StudyTemplateVersioningService>();
        services.AddScoped<SampleCreationDomainService>();
        services.AddScoped<SampleDeletionDomainService>();
        services.AddScoped<StudyCreationDomainService>();

        services.AddScoped<InputParameterServices>();
        services.AddScoped<ResultDefinitionServices>();
        services.AddScoped<StudyTemplateServices>();
        services.AddScoped<CalculationRuleServices>();

        services.AddScoped<OrderServices>();
        services.AddScoped<SampleServices>();
        services.AddScoped<StudyServices>();
        services.AddScoped<TestResultServices>();
        services.AddScoped<MeasuredValueServices>();
    }
}
