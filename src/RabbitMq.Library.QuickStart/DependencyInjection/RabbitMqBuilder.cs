using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RabbitMq.Library.QuickStart.IntegrationEvents;
using RabbitMq.Library.QuickStart.Messages;
using RabbitMq.Library.QuickStart.Receive;

namespace RabbitMq.Library.QuickStart.DependencyInjection;

public sealed class RabbitMqBuilder
{
    private readonly IServiceCollection _services;
    private readonly Dictionary<Type, IntegrationEventDescriptor> _events;
    private readonly Dictionary<Type, IntegrationEventDescriptor> _consumedEvents;
    private bool _receiveInfrastructureRegistered;

    internal RabbitMqBuilder(
        IServiceCollection services,
        Dictionary<Type, IntegrationEventDescriptor> events,
        Dictionary<Type, IntegrationEventDescriptor> consumedEvents)
    {
        _services = services;
        _events = events;
        _consumedEvents = consumedEvents;
    }

    public RabbitMqBuilder AddMessage<TMessage>()
        where TMessage : IIntegrationEvent
    {
        var messageType = typeof(TMessage);

        if (_events.ContainsKey(messageType)) return this;

        var attribute = messageType.GetCustomAttributes(typeof(IntegrationEventAttribute), false)
            .Cast<IntegrationEventAttribute>()
            .SingleOrDefault();

        if (attribute is null)
        {
            throw new InvalidOperationException(
                $"Integration event '{messageType.FullName}' is missing [{nameof(IntegrationEventAttribute)}].");
        }

        if (string.IsNullOrWhiteSpace(attribute.QueueName))
        {
            throw new InvalidOperationException($"Integration event '{messageType.FullName}' has an empty queue name.");
        }

        var duplicateQueue =
            _events.Values.FirstOrDefault(x => x.QueueName == attribute.QueueName && x.EventType != messageType);

        if (duplicateQueue is not null)
        {
            throw new InvalidOperationException(
                $"Queue '{attribute.QueueName}' is already registered for event '{duplicateQueue.EventType.FullName}'.");
        }

        _events.Add(messageType, new IntegrationEventDescriptor(messageType, attribute.QueueName));

        return this;
    }

    public RabbitMqBuilder AddMessageHandler<THandler>()
        where THandler : class
    {
        var handlerType = typeof(THandler);

        var interfaceType = handlerType.GetInterfaces()
            .SingleOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IReceiveHandler<>));

        if (interfaceType is null)
        {
            throw new InvalidOperationException(
                $"Type '{handlerType.FullName}' does not implement IReceiveHandler<TMessage>.");
        }

        var messageType = interfaceType.GetGenericArguments()[0];

        AddMessageInternal(messageType);

        _services.AddScoped(interfaceType, handlerType);

        if (_events.TryGetValue(messageType, out var descriptor))
        {
            _consumedEvents.TryAdd(messageType, descriptor);
        }

        RegisterReceiveInfrastructure();

        return this;
    }

    private void AddMessageInternal(
        Type messageType)
    {
        var method = typeof(RabbitMqBuilder).GetMethod(nameof(AddMessage), BindingFlags.Public | BindingFlags.Instance)!
            .MakeGenericMethod(messageType);
        method.Invoke(this, null);
    }

    private void RegisterReceiveInfrastructure()
    {
        if (_receiveInfrastructureRegistered) return;

        _receiveInfrastructureRegistered = true;

        _services.AddSingleton<ReceiveDispatcher>();
        _services.AddSingleton<RabbitMqMessageReceiver>();
        _services.AddHostedService<ReceiveHandlersBackgroundService>();
    }
}
