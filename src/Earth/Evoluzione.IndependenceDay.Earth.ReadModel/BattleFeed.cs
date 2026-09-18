using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Evoluzione.IndependenceDay.Earth.ReadModel;

public sealed class BattleFeed
{
    private readonly ConcurrentDictionary<Guid, Channel<byte>> _listeners = new();

    public void Notify()
    {
        foreach (var listener in _listeners.Values)
            listener.Writer.TryWrite(1);
    }

    public async IAsyncEnumerable<byte> Subscribe(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
    {
        var id = Guid.NewGuid();

        var channel = Channel.CreateBounded<byte>(new BoundedChannelOptions(1)
        {
            FullMode = BoundedChannelFullMode.DropWrite
        });
        _listeners[id] = channel;

        try
        {
            await foreach (var tick in channel.Reader.ReadAllAsync(ct))
                yield return tick;
        }
        finally
        {
            _listeners.TryRemove(id, out _);
        }
    }
}
