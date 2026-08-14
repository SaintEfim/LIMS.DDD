using Guides.Messages;

namespace Guides.DDD.Service.Infrastructure.Messaging;

public sealed class IntegrationEventRegistry(
    IReadOnlyDictionary<Type, IntegrationEventDescriptor> events)
    : IIntegrationEventRegistry
{
    public IntegrationEventDescriptor Get<T>()
        where T : IIntegrationEvent
    {
        var eventType = typeof(T);

        return events.TryGetValue(eventType, out var descriptor)
            ? descriptor
            : throw new InvalidOperationException(
                $"Integration event '{eventType.FullName}' is not registered.");
    }
}
