using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Contracts.Commands.Sagas;
using Evoluzione.IndependenceDay.Contracts.Ids;
using Evoluzione.IndependenceDay.Contracts.World;
using Muflone.CustomTypes;
using Muflone.Messages.Events;
using Muflone.Saga;

namespace Evoluzione.IndependenceDay.Sagas.ShipInterception;

/// <summary>
/// Il processo che porta giu' una nave e restituisce quello che ha preso in prestito.
/// </summary>
/// <remarks>
/// <b>Questa classe e' l'esercizio.</b> Oggi apre il processo e non fa altro: nessun cannone spara,
/// ogni nave tocca terra, e le citta' cadono alla prima ondata.
/// <para>
/// Qui non va una riga di dominio. Non si sa quanti colpi regga una corazzata, quanto duri una
/// ricarica, quale cannone convenga: quelle cose le sa la Terra, ed e' lei a deciderle. Qui c'e'
/// soltanto la condotta di un processo distribuito, cioe' quattro problemi:
/// </para>
/// <list type="number">
/// <item><b>Il silenzio.</b> Un ordine puo' non arrivare, e un ordine non arrivato non produce
/// nessun evento. Non c'e' niente da intercettare: l'unico modo di accorgersene e' il battito.</item>
/// <item><b>Il guasto.</b> Un cannone si inceppa e smette di sparare. Non si ripara da solo.</item>
/// <item><b>La compensazione.</b> Aprire il fuoco impegna un cannone <b>a tempo indeterminato</b>.
/// Quando la nave e' risolta bisogna restituirlo — e bisogna <b>verificare</b> di averlo restituito,
/// perche' anche il cessate il fuoco puo' perdersi.</item>
/// <item><b>La chiusura.</b> Il processo non finisce quando la nave cade. Finisce quando il conto
/// con la Terra e' chiuso, ed e' l'unico punto in cui si puo' sbagliare senza vedere un errore.</item>
/// </list>
/// <para>
/// I comandi che puoi mandare sono tre: <see cref="OpenFire"/>, <see cref="CeaseFire"/> e
/// <see cref="RepairCannon"/>. Gli eventi che puoi ascoltare sono in <c>EVENTI.md</c>, e ognuno va
/// anche registrato — vedi <see cref="ShipInterceptionSaga"/>.
/// </para>
/// </remarks>
public sealed class InterceptionProcess
{
    private static readonly Account Coordinator = new("saga", "Ship Interception");

    private static readonly EarthId Earth = new(Cities.DefenseId);

    /// <summary>Lo stato iniziale di un'intercettazione: quale nave, su quale citta'.</summary>
    public InterceptionState Open(StartShipInterception command) => new()
    {
        CorrelationId = command.CorrelationId,
        ShipId = Guid.Parse(command.ShipId.Value),
        CityId = Guid.Parse(command.TargetCity.Value),
        Status = SagaStatus.InProgress
    };

    /// <summary>
    /// La prima mossa, appena la Terra prende in carico la nave.
    /// </summary>
    /// <remarks>
    /// Il cronometro degli otto secondi parte adesso. Oggi non si fa niente, e infatti la nave
    /// atterra.
    /// </remarks>
    public InterceptionReaction FirstOrder(InterceptionState state) => InterceptionReaction.None;

    /// <summary>
    /// Cosa fare quando arriva un evento.
    /// </summary>
    /// <remarks>
    /// Arrivano qui solo gli eventi che il processo ha dichiarato di ascoltare: aggiungere un ramo a
    /// questo switch non basta, l'evento va anche registrato. Vedi <see cref="ShipInterceptionSaga"/>.
    /// <para>
    /// Lo <paramref name="state"/> si puo' modificare: la saga lo salva subito dopo, prima di
    /// spedire qualunque ordine.
    /// </para>
    /// </remarks>
    public InterceptionReaction React(InterceptionState state, Event @event) => @event switch
    {
        _ => InterceptionReaction.None
    };

    /// <summary>L'ordine di aprire il fuoco su una nave. Quale cannone lo sceglie la Terra.</summary>
    /// <remarks>
    /// Il cannone che risponde continua a sparare finche' non riceve un <see cref="CeaseFire"/>.
    /// Anche dopo che la nave e' caduta.
    /// </remarks>
    public static InterceptionReaction Fire(InterceptionState state) =>
        InterceptionReaction.Order(
            new OpenFire(Earth, new ShipId(state.ShipId), state.CorrelationId, Coordinator));
}
