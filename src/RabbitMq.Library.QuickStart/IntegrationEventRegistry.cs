using RabbitMq.Library.QuickStart.Abstractions;
using RabbitMq.Library.QuickStart.Messages;

namespace RabbitMq.Library.QuickStart;

public sealed class IntegrationEventRegistry(IReadOnlyDictionary<Type, IntegrationEventDescriptor> events)
    : IIntegrationEventRegistry
{
    public IntegrationEventDescriptor Get<T>()
        where T : IIntegrationEvent
    {
        var eventType = typeof(T);

        return events.TryGetValue(eventType, out var descriptor)
            ? descriptor
            : throw new InvalidOperationException($"Integration event '{eventType.FullName}' is not registered.");
    }
}
