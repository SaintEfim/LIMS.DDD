using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RabbitMq.Library.QuickStart.Abstractions;
using RabbitMq.Library.QuickStart.Connection;
using RabbitMq.Library.QuickStart.Events;
using RabbitMq.Library.QuickStart.Messages;
using RabbitMq.Library.QuickStart.Publishing;
using RabbitMq.Library.QuickStart.Registry;
using RabbitMq.Library.QuickStart.Topology;

namespace RabbitMq.Library.QuickStart.DependencyInjection;

public static class DependencyInjection
{
    public static void AddRabbitMq(
        this IServiceCollection services,
        Assembly assembly)
    {
        var events = DiscoverIntegrationEvents(assembly);

        services.AddSingleton(events);
        services.AddSingleton<IIntegrationEventRegistry, IntegrationEventRegistry>();
        services.AddSingleton<RabbitMqTopologyDeclarator>();
        services.AddSingleton<RabbitMqConnectionProvider>();
        services.AddSingleton<RabbitMqChannelFactory>();
        services.AddHostedService<RabbitMqConnectionMonitor>();
        services.AddScoped<IMessageBus, RabbitMqMessageBus>();
    }

    private static IReadOnlyDictionary<Type, IntegrationEventDescriptor> DiscoverIntegrationEvents(
        Assembly assembly)
    {
        var events = assembly.GetTypes()
            .Where(x => x is { IsClass: true, IsPublic: true, IsAbstract: false } &&
                        typeof(IIntegrationEvent).IsAssignableFrom(x))
            .Select(x =>
            {
                var attribute = x.GetCustomAttribute<IntegrationEventAttribute>();

                return new IntegrationEventDescriptor(x,
                    attribute?.QueueName ??
                    throw new InvalidOperationException(
                        $"Integration event '{x.FullName}' is missing [IntegrationEvent] attribute."));
            })
            .ToDictionary(x => x.EventType, x => x);

        return events;
    }
}
