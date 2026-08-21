using Microsoft.Extensions.DependencyInjection;
using RabbitMq.Library.QuickStart.Abstractions;
using RabbitMq.Library.QuickStart.Connection;
using RabbitMq.Library.QuickStart.IntegrationEvents;

namespace RabbitMq.Library.QuickStart.DependencyInjection;

public static class DependencyInjection
{
    public static RabbitMqBuilder AddRabbitMq(
        this IServiceCollection services,
        Action<RabbitMqOptions> options,
        string serviceName)
    {
        var events = new RegisteredEventsDictionary(new Dictionary<Type, IntegrationEventDescriptor>());
        var consumedEvents = new ConsumedEventsDictionary(new Dictionary<Type, IntegrationEventDescriptor>());

        var rabbitOptions = new RabbitMqOptions();
        options(rabbitOptions);

        services.AddSingleton(rabbitOptions);

        services.AddSingleton<IntegrationEventRegistry>(_ => new IntegrationEventRegistry(events, consumedEvents));

        services.AddSingleton<RabbitMqConnectionProvider>();
        services.AddSingleton<RabbitMqChannelFactory>();
        services.AddSingleton<RabbitMqTopologyDeclarator>();
        services.AddHostedService<RabbitMqConnectionMonitor>();
        services.AddScoped<IMessageBus, RabbitMqMessageBus>();

        return new RabbitMqBuilder(services, events, consumedEvents, serviceName);
    }
}
