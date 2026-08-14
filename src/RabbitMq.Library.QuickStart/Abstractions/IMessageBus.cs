using RabbitMq.Library.QuickStart.Messages;

namespace RabbitMq.Library.QuickStart.Abstractions;

public interface IMessageBus
{
    Task SendAsync<T>(
        T message,
        CancellationToken cancellationToken = default)
        where T : IIntegrationEvent;
}
