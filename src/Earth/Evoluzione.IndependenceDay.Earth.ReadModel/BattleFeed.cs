using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Evoluzione.IndependenceDay.Earth.ReadModel;

/// <summary>
/// La campanella che avvisa le pagine aperte che qualcosa e' cambiato.
/// </summary>
/// <remarks>
/// Non trasporta i dati, solo il fatto che ci sia qualcosa di nuovo: chi ascolta rilegge lo snapshot
/// e lo rimanda intero. Un delta sarebbe piu' magro sul filo, ma andrebbe tenuto allineato a mano con
/// ogni nuova proiezione, e una pagina che perde un delta resta sbagliata fino al ricaricamento.
/// Lo snapshot intero e' cinque citta' e qualche decina di righe: non vale il rischio.
/// <para>
/// Vive in memoria, quindi vale per le pagine collegate a <b>questa</b> istanza. Con piu' repliche
/// della Terra servirebbe un canale condiviso — oggi la replica e' una, e non c'e' niente da risolvere.
/// </para>
/// </remarks>
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
        // Capacita' 1 con DropWrite: a chi e' rimasto indietro non serve la coda degli avvisi persi,
        // gli basta sapere che deve rileggere.
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
