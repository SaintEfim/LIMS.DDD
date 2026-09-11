using Library.Broker.Messages;
using Library.Broker.RabbitMq.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Guides.Service.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddRabbitMq(options =>
            {
                options.HostName = "localhost";
                options.Port = 5672;
                options.UserName = "guest";
                options.Password = "guest";
            }, "guid-service")
            .AddMessage<UnitCreatedMessage>()
            .AddOutbox();
    }
}
