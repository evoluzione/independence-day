using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Contracts.Commands.Sagas;
using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Contracts.Ids;
using Evoluzione.IndependenceDay.Contracts.World;
using Microsoft.Extensions.Logging;
using Muflone.CustomTypes;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.Persistence;
using Muflone.Saga;
using Muflone.Saga.Persistence;

namespace Evoluzione.IndependenceDay.Sagas.ShipInterception;

/// <summary>
/// Porta giu' una nave, e restituisce i cannoni che ha preso in prestito.
/// </summary>
/// <remarks>
/// <b>Questa classe e' l'esercizio.</b> Oggi apre il processo e non ascolta niente: nessun cannone
/// spara, ogni nave tocca terra, e le cinque citta' cadono alla prima ondata.
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
/// con la Terra e' chiuso, ed e' l'unico punto in cui si sbaglia senza vedere un errore.</item>
/// </list>
/// <para>
/// Gli handler qui sotto ci sono gia' tutti, e sono l'elenco di quello che puo' andare storto: nessun
/// cannone libero, cannone inceppato, cannone a secco, cannone ancora acceso su una nave che non c'e'
/// piu'. Piu' il battito, che e' l'unico modo di accorgersi di quello che <b>non</b> e' arrivato.
/// </para>
/// <para>
/// Nessuno di loro fa niente. E nessuno di loro riceve niente, perche' mancano anche le due
/// registrazioni per evento in <c>Sagas.Facade/ExtensionsHelper.cs</c> — dimenticarle non da' errore:
/// l'evento semplicemente non arriva mai, e il processo resta fermo sul gradino precedente.
/// </para>
/// </remarks>
public sealed class ShipInterceptionSaga(
    IServiceBus serviceBus,
    ISagaRepository repository,
    ISagaStateLocator stateLocator,
    ILoggerFactory loggerFactory)
    : LifecycleSaga<StartShipInterception, InterceptionState>(serviceBus, repository, stateLocator, loggerFactory),
        ISagaEventHandlerAsync<ShipApproaching>,
        ISagaEventHandlerAsync<FireOpened>,
        ISagaEventHandlerAsync<FireCeased>,
        ISagaEventHandlerAsync<NoCannonReady>,
        ISagaEventHandlerAsync<CannonJammed>,
        ISagaEventHandlerAsync<CannonRepaired>,
        ISagaEventHandlerAsync<CannonEmpty>,
        ISagaEventHandlerAsync<CannonStillFiring>,
        ISagaEventHandlerAsync<ShipDestroyed>,
        ISagaEventHandlerAsync<ShipLanded>
{
    private static readonly Account Coordinator = new("saga", "Ship Interception");

    private static readonly EarthId Earth = new(Cities.DefenseId);

    /// <summary>
    /// La Terra ha preso in carico una nave: da qui accetta ordini di fuoco, e il cronometro degli
    /// otto secondi e' partito.
    /// </summary>
    /// <remarks>
    /// Lo stato viene salvato, e poi non succede piu' niente. E' il primo posto da guardare.
    /// </remarks>
    public override Task StartedByAsync(StartShipInterception command) =>
        SaveState(command.CorrelationId, new InterceptionState
        {
            CorrelationId = command.CorrelationId,
            ShipId = Guid.Parse(command.ShipId.Value),
            CityId = Guid.Parse(command.TargetCity.Value),
            Status = SagaStatus.InProgress
        });

    // --- quello che succede alla nave -------------------------------------------------------------

    /// <summary>Il battito: la nave e' ancora viva, e <c>CannonsFiring</c> dice quanti le sparano.</summary>
    public Task HandleAsync(ShipApproaching @event) => Advance(@event, _ => []);

    public Task HandleAsync(ShipDestroyed @event) => Advance(@event, _ => []);

    public Task HandleAsync(ShipLanded @event) => Advance(@event, _ => []);

    // --- quello che succede ai cannoni ------------------------------------------------------------

    /// <summary>Un cannone ha aperto il fuoco su questa nave: da adesso e' un debito.</summary>
    public Task HandleAsync(FireOpened @event) => Advance(@event, _ => []);

    /// <summary>Quel cannone e' tornato alla Terra: il debito e' saldato.</summary>
    public Task HandleAsync(FireCeased @event) => Advance(@event, _ => []);

    public Task HandleAsync(NoCannonReady @event) => Advance(@event, _ => []);

    public Task HandleAsync(CannonJammed @event) => Advance(@event, _ => []);

    public Task HandleAsync(CannonRepaired @event) => Advance(@event, _ => []);

    public Task HandleAsync(CannonEmpty @event) => Advance(@event, _ => []);

    /// <summary>Quel cannone spara ancora a una nave che non c'e' piu'.</summary>
    public Task HandleAsync(CannonStillFiring @event) => Advance(@event, _ => []);

    // --- il giro di ogni evento -------------------------------------------------------------------

    /// <summary>
    /// Carica lo stato, scarta le riconsegne, decide, salva, spedisce.
    /// </summary>
    /// <remarks>
    /// L'ordine delle ultime due righe conta. Lo stato si salva <b>prima</b> di spedire: un comando
    /// spedito per primo puo' tornare indietro come evento mentre lo stato e' ancora quello vecchio,
    /// e il processo risponderebbe a se stesso sulla base di cose che non si e' ancora segnato.
    /// <para>
    /// Lo scarto delle riconsegne non e' un dettaglio: senza, lo stesso evento arrivato due volte
    /// aprirebbe due volte il fuoco, e due cannoni finirebbero sulla stessa nave.
    /// </para>
    /// <para>
    /// Manca ancora una cosa, ed e' quella su cui si perde la partita: <b>quando si chiude</b>. Una
    /// saga che non chiama mai <c>CompleteSaga</c> o <c>FailSaga</c> resta aperta per sempre; una che
    /// li chiama troppo presto lascia dei cannoni accesi.
    /// </para>
    /// </remarks>
    private async Task Advance(Event @event, Func<InterceptionState, List<Command>> decide)
    {
        var correlationId = @event.Headers.CorrelationId;

        var state = await LoadState(correlationId);
        if (state is null || !TryAcceptEvent(state, @event))
            return;

        var orders = decide(state);

        await SaveState(correlationId, state);

        foreach (var order in orders)
            await SendCommand(order, correlationId);
    }

    /// <summary>Apri il fuoco su questa nave. Quale cannone lo sceglie la Terra.</summary>
    /// <remarks>
    /// Il cannone che risponde continua a sparare finche' non riceve un <see cref="CeaseFire"/>.
    /// Anche dopo che la nave e' caduta.
    /// </remarks>
    private static Command OpenFire(InterceptionState state) =>
        new OpenFire(Earth, new ShipId(state.ShipId), state.CorrelationId, Coordinator);

    /// <summary>Spegni quel cannone e restituiscilo.</summary>
    private static Command CeaseFire(InterceptionState state, CityId cityId) =>
        new CeaseFire(Earth, cityId, new ShipId(state.ShipId), state.CorrelationId, Coordinator);

    private static Guid Id(CityId cityId) => Guid.Parse(cityId.Value);
}
