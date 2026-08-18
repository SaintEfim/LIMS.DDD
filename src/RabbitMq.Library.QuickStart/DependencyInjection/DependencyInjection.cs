using Microsoft.Extensions.DependencyInjection;
using RabbitMq.Library.QuickStart.Abstractions;
using RabbitMq.Library.QuickStart.Connection;

namespace RabbitMq.Library.QuickStart.DependencyInjection;

public static class DependencyInjection
{
    public static RabbitMqBuilder AddRabbitMq(
        this IServiceCollection services,
        Action<RabbitMqOptions> options)
    {
        var events = new Dictionary<Type, IntegrationEventDescriptor>();
        var consumedEvents = new Dictionary<Type, IntegrationEventDescriptor>();

        var rabbitOptions = new RabbitMqOptions();
        options(rabbitOptions);

        services.AddSingleton(rabbitOptions);

        services.AddSingleton<IntegrationEventRegistry>(sp =>
            new IntegrationEventRegistry(events, consumedEvents));

        services.AddSingleton<RabbitMqConnectionProvider>();
        services.AddSingleton<RabbitMqChannelFactory>();
        services.AddSingleton<RabbitMqTopologyDeclarator>();
        services.AddHostedService<RabbitMqConnectionMonitor>();
        services.AddScoped<IMessageBus, RabbitMqMessageBus>();

        return new RabbitMqBuilder(services, events, consumedEvents);
    }

    public sealed class RegisteredEventsDictionary(
        Dictionary<Type, IntegrationEventDescriptor> dictionary)
        : IReadOnlyDictionary<Type, IntegrationEventDescriptor>
    {
        public IntegrationEventDescriptor this[Type key] => dictionary[key];
        public IEnumerable<Type> Keys => dictionary.Keys;
        public IEnumerable<IntegrationEventDescriptor> Values => dictionary.Values;
        public int Count => dictionary.Count;
        public bool ContainsKey(Type key) => dictionary.ContainsKey(key);
        public bool TryGetValue(Type key, out IntegrationEventDescriptor value)
            => dictionary.TryGetValue(key, out value!);
        public IEnumerator<KeyValuePair<Type, IntegrationEventDescriptor>> GetEnumerator()
            => dictionary.GetEnumerator();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();
    }

    public sealed class ConsumedEventsDictionary(Dictionary<Type, IntegrationEventDescriptor> dictionary)
        : IReadOnlyDictionary<Type, IntegrationEventDescriptor>
    {
        public IntegrationEventDescriptor this[
            Type key] =>
            dictionary[key];

        public IEnumerable<Type> Keys => dictionary.Keys;
        public IEnumerable<IntegrationEventDescriptor> Values => dictionary.Values;
        public int Count => dictionary.Count;

        public bool ContainsKey(
            Type key) =>
            dictionary.ContainsKey(key);

        public bool TryGetValue(
            Type key,
            out IntegrationEventDescriptor value) =>
            dictionary.TryGetValue(key, out value!);

        public IEnumerator<KeyValuePair<Type, IntegrationEventDescriptor>> GetEnumerator() =>
            dictionary.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
