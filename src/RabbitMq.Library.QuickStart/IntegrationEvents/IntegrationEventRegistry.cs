using RabbitMq.Library.QuickStart.Messages;

namespace RabbitMq.Library.QuickStart.IntegrationEvents;

public sealed class IntegrationEventRegistry
{
    private readonly ConsumedEventsDictionary _consumedEvents;
    private readonly RegisteredEventsDictionary _events;

    public IntegrationEventRegistry(
        RegisteredEventsDictionary events,
        ConsumedEventsDictionary consumedEvents)
    {
        _events = events;
        _consumedEvents = consumedEvents;
    }

    public IReadOnlyDictionary<Type, IntegrationEventDescriptor> All => _events;

    public IReadOnlyDictionary<Type, IntegrationEventDescriptor> Consumed => _consumedEvents;

    public IntegrationEventDescriptor Get<T>()
        where T : IIntegrationEvent
    {
        var eventType = typeof(T);
        return _events.TryGetValue(eventType, out var descriptor)
            ? descriptor
            : throw new InvalidOperationException($"Integration event '{eventType.FullName}' is not registered.");
    }
}
