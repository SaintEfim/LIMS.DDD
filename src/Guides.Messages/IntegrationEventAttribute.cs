namespace Guides.Messages;

[AttributeUsage(AttributeTargets.Class)]
public sealed class IntegrationEventAttribute(string queueName) : Attribute
{
    public string QueueName { get; } = queueName;
}
