using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.ReadModel;
using Muflone.Messages.Commands;

namespace Evoluzione.IndependenceDay.Earth.Facade.Messaging;

/// <summary>
/// Il collegamento su cui arrivano gli ordini da fuori. Ogni tanto perde qualcosa.
/// </summary>
/// <remarks>
/// Sta qui, sul bordo, e non dentro il dominio: un aggregato che ricevesse un comando valido e
/// decidesse di ignorarlo sarebbe una rete che finge. Un aggregato non scarta mai un ordine che ha
/// ancora senso — quelli che si perdono non ci arrivano proprio.
/// <para>
/// Vale solo per i tre ordini che vengono da chi coordina. Quello che la Terra manda a se stessa —
/// il grilletto, la scadenza, l'avvistamento — non passa da nessuna radio.
/// </para>
/// </remarks>
public sealed class RadioLink
{
    private int _orders;

    /// <summary>
    /// Se questo ordine arriva a destinazione.
    /// </summary>
    /// <remarks>
    /// Il contatore e' in memoria e non nell'event store, ed e' giusto cosi': quanti messaggi ha
    /// perso un collegamento non e' un fatto di dominio. Un riavvio del servizio lo azzera, e non
    /// cambia niente per chi gioca.
    /// </remarks>
    public bool Delivers() => Radio.Delivers(Interlocked.Increment(ref _orders));
}

/// <summary>
/// Passa un ordine alla Terra, oppure lo lascia cadere.
/// </summary>
/// <remarks>
/// Quando cade non succede niente: niente evento, niente errore, niente risposta. Chi l'ha mandato
/// se ne accorge solo dal battito, che continua a dire che quella nave e' ancora viva e che non le
/// spara nessuno.
/// <para>
/// La riga nel diario e' l'unica traccia, e non esce dalla Terra: serve a chi, a partita finita, si
/// chiede perche' quella citta' fosse scoperta.
/// </para>
/// </remarks>
public sealed class OverRadio<TCommand, THandler>(
    THandler handler,
    RadioLink radio,
    IBattleService battle,
    BattleFeed feed,
    ILogger<OverRadio<TCommand, THandler>> logger) : ICommandHandlerAsync<TCommand>
    where TCommand : Command
    where THandler : ICommandHandlerAsync<TCommand>
{
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        if (radio.Delivers())
        {
            await handler.HandleAsync(command, cancellationToken);
            return;
        }

        logger.LogInformation("[Earth] Ordine perso per strada: {Order}", typeof(TCommand).Name);

        await battle.Log(Guid.Empty, "order-lost", $"ordine perso: {Names[typeof(TCommand).Name]}", Guid.Empty,
            "warn", DateTime.UtcNow, cancellationToken);

        feed.Notify();
    }

    /// <summary>Non tiene niente di proprio: quello che c'e' da liberare e' del gestore avvolto.</summary>
    public void Dispose() => handler.Dispose();

    private static readonly Dictionary<string, string> Names = new()
    {
        ["OpenFire"] = "apertura del fuoco",
        ["CeaseFire"] = "cessate il fuoco",
        ["RepairCannon"] = "riparazione"
    };
}
