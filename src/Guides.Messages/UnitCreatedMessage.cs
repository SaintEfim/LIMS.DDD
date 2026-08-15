using RabbitMq.Library.QuickStart.Messages;

namespace Guides.Messages;

[IntegrationEvent("unit.created")]
public record UnitCreatedMessage(string Name) : IIntegrationEvent;
