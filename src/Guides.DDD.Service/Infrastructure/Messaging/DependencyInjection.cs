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

        services.AddSingleton<IReadOnlyCollection<IntegrationEventDescriptor>>(events);
        services.AddSingleton<RabbitMqTopologyDeclarator>();
        services.AddSingleton<RabbitMqConnectionProvider>();
        services.AddSingleton<RabbitMqChannelManager>();
        services.AddHostedService<RabbitMqConnectionMonitor>();
        services.AddScoped<IMessageBus, RabbitMqMessageBus>();
    }

    private static IntegrationEventDescriptor[] DiscoverIntegrationEvents(
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
            .ToArray();

        var duplicates = events.GroupBy(e => e.QueueName)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToArray();

        if (duplicates.Length > 0)
        {
            throw new InvalidOperationException($"Duplicate queue names detected: [{string.Join(", ", duplicates)}]. " +
                                                "Each integration event must have a unique queue name.");
        }

        return events;
    }
}
