using System.Threading.Channels;

namespace RabbitMq.Library.Outbox;

public sealed class OutboxSignal
{
    private readonly Channel<bool> _channel = Channel.CreateBounded<bool>(new BoundedChannelOptions(1)
    {
        FullMode = BoundedChannelFullMode.DropWrite,
        SingleReader = true,
        SingleWriter = false
    });

    public void Notify()
    {
        _channel.Writer.TryWrite(true);
    }

    public async ValueTask WaitAsync(
        CancellationToken cancellationToken = default)
    {
        await _channel.Reader.ReadAsync(cancellationToken);
    }
}
