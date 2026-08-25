namespace RabbitMq.Library.Broker.IntegrationEvents;

public sealed record IntegrationEventDescriptor(Type EventType, string ExchangeName, string QueueName);
