using Guides.Messages;
using Microsoft.Extensions.DependencyInjection;
using RabbitMq.Library.QuickStart.DependencyInjection;

namespace LIMS.Service.LaboratoryOperations.Infrastructure;

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
            }, typeof(UnitCreatedMessage).Assembly)
            .AddMessageHandler<UnitCreatedMessageHandler>()
            .Build();
    }
}
