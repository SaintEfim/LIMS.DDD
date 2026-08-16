using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RabbitMq.Library.QuickStart.Abstractions;
using RabbitMq.Library.QuickStart.Connection;
using RabbitMq.Library.QuickStart.Messages;

namespace RabbitMq.Library.QuickStart.DependencyInjection;

public static class DependencyInjection
{
    public static RabbitMqBuilder AddRabbitMq(
        this IServiceCollection services,
        Action<RabbitMqOptions> options,
        params Assembly[] assemblies)
    {
        var events = DiscoverIntegrationEvents(assemblies);
        services.AddSingleton(events);
        services.AddSingleton<IntegrationEventRegistry>();

        var rabbitOptions = new RabbitMqOptions();
        options(rabbitOptions);
        services.AddSingleton(rabbitOptions);

        services.AddSingleton<RabbitMqConnectionProvider>();
        services.AddSingleton<RabbitMqChannelFactory>();
        services.AddSingleton<RabbitMqTopologyDeclarator>();
        services.AddHostedService<RabbitMqConnectionMonitor>();
        services.AddScoped<IMessageBus, RabbitMqMessageBus>();

        return new RabbitMqBuilder(services);
    }

    private static IReadOnlyDictionary<Type, IntegrationEventDescriptor> DiscoverIntegrationEvents(
        Assembly[] assemblyMarkers)
    {
        if (assemblyMarkers.Length == 0)
        {
            throw new ArgumentException("At least one assembly must be provided for integration event discovery.",
                nameof(assemblyMarkers));
        }

        return assemblyMarkers.SelectMany(a => a.GetTypes())
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
