using Muflone.Messages.Commands;

namespace Evoluzione.IndependenceDay.Sagas.ShipInterception;

/// <summary>Come finisce un'intercettazione, o se deve continuare.</summary>
public enum InterceptionOutcome
{
    /// <summary>C'e' ancora qualcosa da fare o da aspettare.</summary>
    Continue = 0,

    /// <summary>Nave abbattuta e cannoni tutti restituiti: si chiude bene.</summary>
    Won = 1,

    /// <summary>Nave a terra: si chiude male, e resta su Mongo a dirlo.</summary>
    Lost = 2
}

/// <summary>
/// Quello che il processo deve fare dopo un evento: quali ordini impartire, e se chiudere.
/// </summary>
/// <remarks>
/// E' il confine fra chi conduce il processo e chi lo esegue. <see cref="InterceptionProcess"/>
/// produce una reazione e non tocca ne' il bus ne' la persistenza; la saga la esegue senza sapere
/// perche'. Cosi' il processo si prova con un test che non accende niente, e la saga resta senza rami.
/// </remarks>
public sealed record InterceptionReaction(
    IReadOnlyList<Command> Orders,
    InterceptionOutcome Outcome,
    string Reason = "")
{
    /// <summary>Nessun ordine, processo aperto: si resta ad aspettare il prossimo evento.</summary>
    public static readonly InterceptionReaction None = new([], InterceptionOutcome.Continue);

    /// <summary>Uno o piu' ordini, e si resta in ascolto dell'esito.</summary>
    public static InterceptionReaction Order(params Command[] orders) =>
        new(orders, InterceptionOutcome.Continue);

    /// <summary>Nave abbattuta e conto chiuso con la Terra.</summary>
    public static readonly InterceptionReaction Won = new([], InterceptionOutcome.Won);

    /// <summary>Nave a terra, e il perche' resta scritto.</summary>
    public static InterceptionReaction Lost(string reason) => new([], InterceptionOutcome.Lost, reason);
}
