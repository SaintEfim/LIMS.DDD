namespace RabbitMq.Library.QuickStart.Events;

public sealed record IntegrationEventDescriptor(
    Type EventType,
    string QueueName);
