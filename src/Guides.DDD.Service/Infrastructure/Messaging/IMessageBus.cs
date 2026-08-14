using Guides.Messages;

namespace Guides.DDD.Service.Infrastructure.Messaging;

public interface IMessageBus
{
    Task SendAsync<T>(
        T message,
        CancellationToken cancellationToken = default)
        where T : IIntegrationEvent;
}
