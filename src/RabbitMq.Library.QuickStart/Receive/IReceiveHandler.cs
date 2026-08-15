using RabbitMq.Library.QuickStart.Messages;

namespace RabbitMq.Library.QuickStart.Receive;

public interface IReceiveHandler<in TMessage>
    where TMessage : IIntegrationEvent
{
    Task HandleAsync(
        TMessage message,
        CancellationToken cancellationToken = default);
}
