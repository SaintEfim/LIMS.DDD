using RabbitMq.Library.QuickStart.Messages;

namespace Guides.Messages;

[IntegrationEvent("unit.created")]
public record UnitCreatedMessage(Guid Id, string Name) : IIntegrationEvent;
