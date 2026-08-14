using RabbitMq.Library.QuickStart.Messages;

namespace RabbitMq.Library.QuickStart.Abstractions;

public interface IIntegrationEventRegistry
{
    IntegrationEventDescriptor Get<T>()
        where T : IIntegrationEvent;
}
