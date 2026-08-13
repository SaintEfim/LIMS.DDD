namespace Guides.DDD.Service.Infrastructure.Messaging;

public interface IMessageBus
{
    Task Send(
        object obj,
        CancellationToken cancellationToken = default);

    Task Send(
        string message,
        CancellationToken cancellationToken = default);
}
