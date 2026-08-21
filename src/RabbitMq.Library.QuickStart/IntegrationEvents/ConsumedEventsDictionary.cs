using System.Collections;

namespace RabbitMq.Library.QuickStart.IntegrationEvents;

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
        Type key)
    {
        return dictionary.ContainsKey(key);
    }

    public bool TryGetValue(
        Type key,
        out IntegrationEventDescriptor value)
    {
        return dictionary.TryGetValue(key, out value!);
    }

    public IEnumerator<KeyValuePair<Type, IntegrationEventDescriptor>> GetEnumerator()
    {
        return dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void TryAdd(
        Type messageType,
        IntegrationEventDescriptor descriptor)
    {
        dictionary.TryAdd(messageType, descriptor);
    }
}
