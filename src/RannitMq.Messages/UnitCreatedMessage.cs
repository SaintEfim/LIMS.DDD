using RabbitMq.Library.QuickStart.Messages;

namespace RannitMq.Messages;

[IntegrationEvent("unit.created")]
public record UnitCreatedMessage(Guid Id, string Name) : IIntegrationEvent;
