using Microsoft.Extensions.DependencyInjection;

namespace RabbitMq.Library.Broker.Receive;

public sealed class ReceiveDispatcher(IServiceScopeFactory scopeFactory)
{
    public async Task DispatchAsync(
        Type messageType,
        object message,
        CancellationToken cancellationToken = default)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var handlerType = typeof(IReceiveHandler<>).MakeGenericType(messageType);
        var handler = scope.ServiceProvider.GetRequiredService(handlerType);
        var method = handlerType.GetMethod(nameof(IReceiveHandler<>.HandleAsync));
        if (method is null)
        {
            throw new InvalidOperationException($"Handler for '{messageType.Name}' " +
                                                $"does not contain HandleAsync.");
        }

        var task = (Task?) method.Invoke(handler, [message, cancellationToken]);
        if (task is not null)
        {
            await task;
        }
    }
}
