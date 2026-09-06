using RabbitMq.Library.Broker;

namespace Library.Broker.Messages;

[IntegrationEvent("unit.created")]
public record UnitCreatedMessage(Guid Id, string Name) : IIntegrationEvent;
