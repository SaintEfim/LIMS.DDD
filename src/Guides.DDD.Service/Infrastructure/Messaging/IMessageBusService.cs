namespace Guides.DDD.Service.Infrastructure.Messaging;

public interface IMessageBusService
{
    Task SendMessage(
        object obj,
        CancellationToken cancellationToken = default);

    Task SendMessage(
        string message,
        CancellationToken cancellationToken = default);
}
