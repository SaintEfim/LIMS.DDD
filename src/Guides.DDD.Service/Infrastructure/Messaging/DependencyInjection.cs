using System.Reflection;
using Guides.Messages;

namespace Guides.DDD.Service.Infrastructure.Messaging;

public static class DependencyInjection
{
    public static void AddRabbitMq(
        this IServiceCollection services,
        Assembly assembly)
    {
        var events = assembly.GetTypes()
            .Where(x => x is { IsClass: true, IsPublic: true } && typeof(IIntegrationEvent).IsAssignableFrom(x))
            .Select(x =>
            {
                var attribute = x.GetCustomAttribute<IntegrationEventAttribute>();

                return new IntegrationEventDescriptor(x,
                    attribute?.QueueName ??
                    throw new InvalidOperationException(
                        $"Integration event {x.Name} has no IntegrationEventAttribute."));
            })
            .ToArray();

        services.AddSingleton<IReadOnlyCollection<IntegrationEventDescriptor>>(events);

        services.AddSingleton<RabbitMqTopologyDeclarator>();
        services.AddSingleton<RabbitMqConnectionProvider>();
        services.AddSingleton<RabbitMqChannelManager>();
        services.AddHostedService<RabbitMqConnectionMonitor>();
        services.AddScoped<IMessageBus, RabbitMqMessageBus>();
    }
}
