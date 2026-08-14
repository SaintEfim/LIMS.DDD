namespace Guides.Messages;

[IntegrationEvent("unit.created")]
public record UnitCreated(string Name) : IIntegrationEvent;
