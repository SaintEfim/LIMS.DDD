namespace RabbitMq.Library.Broker;

[AttributeUsage(AttributeTargets.Class)]
public sealed class IntegrationEventAttribute(string eventName) : Attribute
{
    public string EventName { get; } = eventName;
}
