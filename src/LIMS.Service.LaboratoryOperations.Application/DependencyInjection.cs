using LIMS.Service.LaboratoryOperations.Application.Orders;
using LIMS.Service.LaboratoryOperations.Application.Samples;
using LIMS.Service.LaboratoryOperations.Application.Studies.Core;
using LIMS.Service.LaboratoryOperations.Application.Studies.MeasuredValues;
using LIMS.Service.LaboratoryOperations.Application.Studies.TestResults;
using LIMS.Service.LaboratoryOperations.Application.StudyTemplates;
using LIMS.Service.LaboratoryOperations.Application.Units;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS.Service.LaboratoryOperations.Application;

public static class DependencyInjection
{
    public static void AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<UnitSnapshotCommandsHandler>();

        services.AddScoped<StudyTemplateSnapshotCommandsHandler>();

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
