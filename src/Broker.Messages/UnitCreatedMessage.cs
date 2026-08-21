using RabbitMq.Library.QuickStart.Messages;

namespace Broker.Messages;

[IntegrationEvent("unit.created")]
public record UnitCreatedMessage(Guid Id, string Name) : IIntegrationEvent;
