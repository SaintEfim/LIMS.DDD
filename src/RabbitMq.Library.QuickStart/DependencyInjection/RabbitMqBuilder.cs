using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RabbitMq.Library.QuickStart.Receive;

namespace RabbitMq.Library.QuickStart.DependencyInjection;

public class RabbitMqBuilder
{
    private readonly IServiceCollection _services;
    private readonly List<Assembly> _handlerAssemblies = [];
    private bool _hasConsumers;

    internal RabbitMqBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public RabbitMqBuilder AddMessageHandler<THandler>() where THandler : class
    {
        var handlerType = typeof(THandler);
        var interfaceType = handlerType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType
                              && i.GetGenericTypeDefinition() == typeof(IReceiveHandler<>));

        if (interfaceType is null)
            throw new InvalidOperationException(
                $"Type '{handlerType.Name}' does not implement IReceiveHandler<T>.");

        _services.AddScoped(interfaceType, handlerType);
        _hasConsumers = true;
        return this;
    }

    public RabbitMqBuilder AddMessageHandlersFrom(Assembly assembly)
    {
        _handlerAssemblies.Add(assembly);
        _hasConsumers = true;
        return this;
    }

    public void Build()
    {
        if (!_hasConsumers) return;

        // Сканирование сборок
        foreach (var assembly in _handlerAssemblies)
        {
            var handlerTypes = assembly.GetTypes()
                .Where(t => t is { IsClass: true, IsAbstract: false }
                         && t.GetInterfaces().Any(i =>
                             i.IsGenericType
                             && i.GetGenericTypeDefinition() == typeof(IReceiveHandler<>)));

            foreach (var handlerType in handlerTypes)
            {
                var interfaceType = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType
                             && i.GetGenericTypeDefinition() == typeof(IReceiveHandler<>));

                _services.AddScoped(interfaceType, handlerType);
            }
        }

        // ✅ Consumer-инфраструктура регистрируется ТОЛЬКО при наличии обработчиков
        _services.AddSingleton<ReceiveDispatcher>();
        _services.AddSingleton<RabbitMqMessageReceiver>();
        _services.AddHostedService<ReceiveHandlersBackgroundService>();
    }
}
