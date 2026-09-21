using Microsoft.Extensions.DependencyInjection;
using Library.Broker.RabbitMq.DependencyInjection;
using LIMS.Service.LaboratoryOperations.Application.BackgroundOperations;
using LIMS.Service.LaboratoryOperations.Domain.BackgroundOperations;
using LIMS.Service.LaboratoryOperations.Infrastructure.BackgroundOperations;

namespace LIMS.Service.LaboratoryOperations.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddKeyedScoped<IBackgroundOperationProcessor, ReportProcessor>(OperationType.GenerateReport);
        services.AddRabbitMq(x =>
            {
                x.HostName = "localhost";
                x.Port = 5672;
                x.UserName = "guest";
                x.Password = "guest";
            }, "laboratory-operations")
            .AddMessageHandler<UnitCreatedMessageHandler>()
            .AddMessageHandler<StudyTemplatePublishedMessageHandler>();
    }
}
