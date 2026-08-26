using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMq.Library.Broker.IntegrationEvents;
using RabbitMq.Library.Broker.Receive;
using RabbitMq.Library.Outbox;

namespace RabbitMq.Library.Broker.DependencyInjection;

public sealed class RabbitMqBuilder
{
    private readonly ConsumedEventsDictionary _consumedEvents;
    private readonly RegisteredEventsDictionary _events;

    private readonly string _serviceName;
    private readonly IServiceCollection _services;
    private bool _receiveInfrastructureRegistered;

    internal RabbitMqBuilder(
        IServiceCollection services,
        RegisteredEventsDictionary events,
        ConsumedEventsDictionary consumedEvents,
        string serviceName)
    {
        _services = services;
        _events = events;
        _consumedEvents = consumedEvents;
        _serviceName = serviceName;
    }

    public RabbitMqBuilder AddMessage<TMessage>()
        where TMessage : IIntegrationEvent
    {
        var messageType = typeof(TMessage);

        if (_events.ContainsKey(messageType))
        {
            return this;
        }

        var attribute = messageType.GetCustomAttributes(typeof(IntegrationEventAttribute), false)
            .Cast<IntegrationEventAttribute>()
            .SingleOrDefault();

        if (attribute is null)
        {
            throw new InvalidOperationException(
                $"Integration event '{messageType.FullName}' is missing [{nameof(IntegrationEventAttribute)}].");
        }

        if (string.IsNullOrWhiteSpace(attribute.EventName))
        {
            throw new InvalidOperationException($"Integration event '{messageType.FullName}' has an empty queue name.");
        }

        var duplicateQueue =
            _events.Values.FirstOrDefault(x => x.QueueName == attribute.EventName && x.EventType != messageType);

        if (duplicateQueue is not null)
        {
            throw new InvalidOperationException(
                $"Queue '{attribute.EventName}' is already registered for event '{duplicateQueue.EventType.FullName}'.");
        }

        var exchangeName = attribute.EventName;
        var queueName = $"{attribute.EventName}.{_serviceName}";

        _events.Add(messageType, new IntegrationEventDescriptor(messageType, exchangeName, queueName));

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

    public RabbitMqBuilder AddOutbox<TDbContext>()
        where TDbContext : DbContext
    {
        _services.AddSingleton<OutboxSignal>();

        _services.AddScoped<OutboxSaveChangesInterceptor>();
        _services.AddScoped<OutboxProcessor<TDbContext>>();

        _services.AddHostedService<OutboxBackgroundService<TDbContext>>();

        _services.AddDbContext<TDbContext>((
            serviceProvider,
            options) =>
        {
            options.AddInterceptors(serviceProvider.GetRequiredService<OutboxSaveChangesInterceptor>());
        });

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
        if (_receiveInfrastructureRegistered)
        {
            return;
        }

        _receiveInfrastructureRegistered = true;

        _services.AddSingleton<ReceiveDispatcher>();
        _services.AddSingleton<RabbitMqMessageReceiver>();
        _services.AddHostedService<ReceiveHandlersBackgroundService>();
    }
}
