using Muflone.Messages.Events;

namespace Evoluzione.IndependenceDay.Infrastructure.Messaging;

public static class MessageHelpers
{
    /// <summary>Revisione di un evento che non viene dall'event store: la scrittura non si versiona.</summary>
    public const long Unversioned = -1;


    public static DateTime When(this Event @event) => @event.Headers.When.Value;

    public static Guid CorrelationId(this Event @event) => @event.Headers.CorrelationId;

    /// <summary>
    /// La posizione dell'evento sul log di EventStore, usata dai read model come numero di versione.
    /// </summary>
    /// <remarks>
    /// Si prende la PreparePosition e non la CommitPosition: gli eventi scritti in un unico append
    /// condividono la commit, quindi filtrando su quella tutti tranne il primo verrebbero scartati
    /// come "gia' applicati".
    /// </remarks>
    public static long EventRevision(this Event @event)
    {
        if (@event.Headers.Customs.TryGetValue("EventStorePreparePosition", out var prepare) &&
            long.TryParse(prepare, out var preparePosition))
            return preparePosition;

        if (@event.Headers.Customs.TryGetValue("EventStoreCommitPosition", out var commit) &&
            long.TryParse(commit, out var commitPosition))
            return commitPosition;

        // Un evento che non arriva dalla sottoscrizione all'event store — un evento di integrazione
        // preso dal bus, un test — non ha posizione. Vale -1, che ProjectionPersister riconosce come
        // "scrittura senza versione". Riempirlo con un orologio sarebbe peggio che lasciarlo vuoto:
        // i tick sono ordini di grandezza sopra qualunque posizione reale, e da quel momento in poi
        // ogni scrittura versionata su quel documento verrebbe scartata come vecchia, in silenzio.
        return Unversioned;
    }
}
