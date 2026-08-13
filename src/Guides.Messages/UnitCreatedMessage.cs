namespace Guides.Messages;

public record UnitCreated(string Name) : MessageBase
{
    public override string MessageType => "unit.created";
}
