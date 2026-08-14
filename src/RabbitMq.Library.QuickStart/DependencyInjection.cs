using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RabbitMq.Library.QuickStart.Abstractions;
using RabbitMq.Library.QuickStart.Connection;
using RabbitMq.Library.QuickStart.Messages;

namespace RabbitMq.Library.QuickStart;

public static class DependencyInjection
{
    public static void AddRabbitMq(
        this IServiceCollection services,
        Action<RabbitMqOptions> options,
        Assembly[] assemblies)
    {
        var events = DiscoverIntegrationEvents(assemblies);

        var rabbitOptions = new RabbitMqOptions();
        options(rabbitOptions);
        services.AddSingleton(rabbitOptions);

        services.AddSingleton(events);
        services.AddSingleton<IIntegrationEventRegistry, IntegrationEventRegistry>();
        services.AddSingleton<RabbitMqTopologyDeclarator>();
        services.AddSingleton<RabbitMqConnectionProvider>();
        services.AddSingleton<RabbitMqChannelFactory>();
        services.AddHostedService<RabbitMqConnectionMonitor>();
        services.AddScoped<IMessageBus, RabbitMqMessageBus>();
    }

    private static IReadOnlyDictionary<Type, IntegrationEventDescriptor> DiscoverIntegrationEvents(
        Assembly[] assemblies)
    {
        return assemblies.SelectMany(a => a.GetTypes())
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
