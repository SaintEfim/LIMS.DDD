using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RabbitMq.Library.QuickStart.Abstractions;
using RabbitMq.Library.QuickStart.Connection;
using RabbitMq.Library.QuickStart.Messages;
using RabbitMq.Library.QuickStart.Receive;

namespace RabbitMq.Library.QuickStart;

public static class DependencyInjection
{
    extension(
        IServiceCollection services)
    {
        public IServiceCollection AddRabbitMq(
            Action<RabbitMqOptions> options,
            Type[] assemblyMarkers)
        {
            var events = DiscoverIntegrationEvents(assemblyMarkers);

            var rabbitOptions = new RabbitMqOptions();
            options(rabbitOptions);
            services.AddSingleton(rabbitOptions);

            services.AddSingleton<RabbitMqMessageReceiver>();
            services.AddSingleton(events);
            services.AddSingleton<IntegrationEventRegistry>();
            services.AddSingleton<RabbitMqTopologyDeclarator>();
            services.AddSingleton<RabbitMqConnectionProvider>();
            services.AddSingleton<RabbitMqChannelFactory>();

            services.AddHostedService<RabbitMqConnectionMonitor>();
            services.AddHostedService<ReceiveHandlersBackgroundService>();

            services.AddScoped<IMessageBus, RabbitMqMessageBus>();

            services.AddSingleton<ReceiveDispatcher>();

            return services;
        }

        public IServiceCollection AddMessageHandlers(
            Type[] types)
        {
            var handlerTypes = types.Select(x => x.Assembly)
                .SelectMany(a => a.GetTypes())
                .Where(t => t is { IsClass: true, IsAbstract: false } && t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IReceiveHandler<>)));

            foreach (var handlerType in handlerTypes)
            {
                var interfaceType = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IReceiveHandler<>));

                services.AddScoped(interfaceType, handlerType);
            }

            return services;
        }
    }

    private static IReadOnlyDictionary<Type, IntegrationEventDescriptor> DiscoverIntegrationEvents(
        Type[] assemblyMarkers)
    {
        return assemblyMarkers.Select(x => x.Assembly)
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true, IsPublic: true, IsAbstract: false } &&
                        typeof(IIntegrationEvent).IsAssignableFrom(t))
            .Select(t => new
            {
                Type = t,
                Attribute = t.GetCustomAttribute<IntegrationEventAttribute>() ??
                            throw new InvalidOperationException(
                                $"Integration event '{t.FullName}' is missing [IntegrationEvent] attribute.")
            })
            .ToDictionary(x => x.Type, x => new IntegrationEventDescriptor(x.Type, x.Attribute.QueueName));
    }
}
