namespace RabbitMq.Library.QuickStart;

public sealed record IntegrationEventDescriptor(Type EventType, string QueueName);
