namespace Library.Broker.RabbitMq.IntegrationEvents;

public sealed record IntegrationEventDescriptor(Type EventType, string ExchangeName, string QueueName);
