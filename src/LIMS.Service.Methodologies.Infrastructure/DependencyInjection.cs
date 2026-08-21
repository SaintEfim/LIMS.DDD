using Broker.Messages;
using Microsoft.Extensions.DependencyInjection;
using RabbitMq.Library.QuickStart.DependencyInjection;

namespace LIMS.Service.Methodologies.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddRabbitMq(x =>
            {
                x.HostName = "localhost";
                x.Port = 5672;
                x.UserName = "guest";
                x.Password = "guest";
            }, "methodologies")
            .AddMessage<StudyTemplatePublishedMessage>()
            .AddMessageHandler<UnitCreatedMessageHandler>();
    }
}
