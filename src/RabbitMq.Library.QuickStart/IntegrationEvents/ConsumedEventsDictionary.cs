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
        Type key) =>
        dictionary.ContainsKey(key);

    public bool TryGetValue(
        Type key,
        out IntegrationEventDescriptor value) =>
        dictionary.TryGetValue(key, out value!);

    public IEnumerator<KeyValuePair<Type, IntegrationEventDescriptor>> GetEnumerator() =>
        dictionary.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    public void TryAdd(
        Type messageType,
        IntegrationEventDescriptor descriptor) =>
        dictionary.TryAdd(messageType, descriptor);
}
