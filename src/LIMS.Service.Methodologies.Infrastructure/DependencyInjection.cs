using Guides.Messages;
using Microsoft.Extensions.DependencyInjection;
using RabbitMq.Library.QuickStart.DependencyInjection;

namespace LIMS.Service.Methodologies.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(
        this IServiceCollection services)
    {
        // TODO сделать регистрацию event более внятной
        services.AddRabbitMq(options =>
            {
                options.HostName = "localhost";
                options.Port = 5672;
                options.UserName = "guest";
                options.Password = "guest";
            })
            .AddMessage<StudyTemplatePublishedMessage>()
            .AddMessageHandler<UnitCreatedMessageHandler>();
    }
}
