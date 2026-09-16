using Muflone.Saga;

namespace Evoluzione.IndependenceDay.Sagas.ShipInterception;

/// <summary>Come e' finita la nave, per chi la stava intercettando.</summary>
public enum ShipFate
{
    /// <summary>Ancora in volo.</summary>
    InFlight = 0,

    Destroyed = 1,
    Landed = 2
}

/// <summary>
/// Quello che il processo si ricorda di una nave, fra un evento e il successivo.
/// </summary>
/// <remarks>
/// Tutto quello che si mette qui viene salvato su Mongo a ogni passo e ritrovato al successivo, anche
/// dopo un riavvio del servizio. Ed e' l'unica memoria che c'e': la Terra non risponde a domande, e
/// un processo vive per una nave sola e non vede cosa stanno facendo gli altri.
/// </remarks>
public sealed class InterceptionState : SagaStateBase
{
    /// <summary>La chiave di business: con questa un evento in ritardo ritrova il suo processo.</summary>
    public Guid ShipId { get; set; }

    /// <summary>La citta' che la nave sta puntando. Non e' detto sia quella che le spara.</summary>
    public Guid CityId { get; set; }

    /// <summary>
    /// I cannoni che questo processo ha aperto e non ha ancora chiuso.
    /// </summary>
    /// <remarks>
    /// E' il conto aperto con la Terra, ed e' la ragione per cui il processo non puo' chiudersi
    /// quando la nave cade: finche' qui dentro c'e' qualcosa, c'e' un cannone che spara per conto suo.
    /// </remarks>
    public HashSet<Guid> Firing { get; set; } = [];

    /// <summary>Com'e' finita la nave. Finita la nave, resta da chiudere quello che si e' aperto.</summary>
    public ShipFate Fate { get; set; }
}
