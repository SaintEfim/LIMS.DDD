using Guides.Messages;
using LIMS.DDD.Service.Infrastructure.Units.Receive;
using Microsoft.Extensions.DependencyInjection;
using RabbitMq.Library.QuickStart.DependencyInjection;

namespace LIMS.DDD.Service.Infrastructure;

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
