using RabbitMq.Library.Broker;

namespace Broker.Messages;

[IntegrationEvent("unit.created")]
public record UnitCreatedMessage(Guid Id, string Name) : IIntegrationEvent;
