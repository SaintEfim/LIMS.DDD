namespace Guides.Messages;

public abstract record MessageBase
{
    public abstract string MessageType { get; }
}
