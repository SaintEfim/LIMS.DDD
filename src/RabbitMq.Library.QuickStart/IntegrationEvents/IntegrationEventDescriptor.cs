namespace RabbitMq.Library.QuickStart.IntegrationEvents;

public sealed record IntegrationEventDescriptor(Type EventType, string ExchangeName, string QueueName);
