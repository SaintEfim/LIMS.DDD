using System.Reflection;
using Guides.Messages;

namespace Guides.DDD.Service.Infrastructure.Messaging;

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
