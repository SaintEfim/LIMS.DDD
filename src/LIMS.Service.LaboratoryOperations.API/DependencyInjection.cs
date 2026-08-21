using LIMS.Service.LaboratoryOperations.API.Apis.MeasuredValues;
using LIMS.Service.LaboratoryOperations.API.Apis.Orders;
using LIMS.Service.LaboratoryOperations.API.Apis.Samples;
using LIMS.Service.LaboratoryOperations.API.Apis.Studies;
using LIMS.Service.LaboratoryOperations.API.Apis.StudyTemplateShanpshot;
using LIMS.Service.LaboratoryOperations.API.Apis.TestResults;
using LIMS.Service.LaboratoryOperations.API.Apis.UnitSnapshot;
using LIMS.Service.LaboratoryOperations.Application;
using LIMS.Service.LaboratoryOperations.Domain.Services;
using LIMS.Service.LaboratoryOperations.Infrastructure;
using LIMS.Service.LaboratoryOperations.Persistence;

namespace LIMS.Service.LaboratoryOperations.API;

public static class DependencyInjection
{
    public static void AddApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructure();
        services.AddPersistence(configuration);
        services.AddApplication();

        services.AddScoped<UnitSnapshotServices>();

        services.AddScoped<StudyStatusChangeDomainService>();
        services.AddScoped<SampleStatusChangeDomainService>();
        services.AddScoped<StudyStatusChangeDomainService>();
        services.AddScoped<OrderStatusChangeDomainService>();

        services.AddScoped<SampleCreationDomainService>();
        services.AddScoped<SampleDeletionDomainService>();
        services.AddScoped<StudyCreationDomainService>();

        services.AddScoped<OrderServices>();
        services.AddScoped<SampleServices>();
        services.AddScoped<StudyServices>();
        services.AddScoped<TestResultServices>();
        services.AddScoped<MeasuredValueServices>();
        services.AddScoped<StudyTemplateSnapshotServices>();
        services.AddScoped<TestResultDomainService>();
    }
}
