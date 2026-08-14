namespace Guides.DDD.Service.Infrastructure.Messaging;

public sealed record IntegrationEventDescriptor(
    Type EventType,
    string QueueName);
