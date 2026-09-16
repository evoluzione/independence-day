using Evoluzione.IndependenceDay.Contracts.Commands.Sagas;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;
using Muflone.Persistence;
using Muflone.Saga;
using Muflone.Saga.Persistence;

namespace Evoluzione.IndependenceDay.Sagas.ShipInterception;

/// <summary>
/// Il coordinamento dell'intercettazione: riceve, chiede a chi conduce, esegue.
/// </summary>
/// <remarks>
/// Questa classe non contiene decisioni: niente conti, niente rami, niente <c>if</c>. Carica lo
/// stato, scarta le riconsegne, passa l'evento a <see cref="InterceptionProcess"/> e fa quello che la
/// reazione dice. Quello che si legge qui e' <b>quali eventi attraversano il processo</b>; il perche'
/// sta nel processo.
/// <para>
/// Oggi non ne attraversa nessuno. Per ascoltarne uno servono tre righe, e vanno tutte e tre:
/// </para>
/// <list type="number">
/// <item><c>ISagaEventHandlerAsync&lt;TEvento&gt;</c> nell'elenco qui sotto;</item>
/// <item><c>public Task HandleAsync(TEvento @event) =&gt; Advance(@event);</c></item>
/// <item>le due registrazioni in <c>Sagas.Facade/ExtensionsHelper.cs</c>.</item>
/// </list>
/// <para>
/// Dimenticare la terza non da' errore: l'evento semplicemente non arriva mai, e il processo resta
/// fermo sul gradino precedente.
/// </para>
/// </remarks>
public sealed class ShipInterceptionSaga(
    IServiceBus serviceBus,
    ISagaRepository repository,
    ISagaStateLocator stateLocator,
    ILoggerFactory loggerFactory)
    : LifecycleSaga<StartShipInterception, InterceptionState>(serviceBus, repository, stateLocator, loggerFactory)
{
    private readonly InterceptionProcess _process = new();

    /// <summary>
    /// La Terra ha preso in carico una nave: da qui accetta ordini di fuoco, e il cronometro degli
    /// otto secondi e' partito.
    /// </summary>
    public override Task StartedByAsync(StartShipInterception command)
    {
        var state = _process.Open(command);

        return Carry(command.CorrelationId, state, _process.FirstOrder(state));
    }

    /// <summary>
    /// Il giro di ogni evento: stato, decisione, esecuzione.
    /// </summary>
    /// <remarks>
    /// I processi da far avanzare sono zero o uno — puo' essere gia' chiuso, o l'evento essere una
    /// riconsegna del broker — e scorrerli invece di interrogarli e' quello che tiene questo metodo
    /// senza rami. Senza lo scarto delle riconsegne lo stesso evento arrivato due volte aprirebbe due
    /// volte il fuoco, e due cannoni finirebbero sulla stessa nave.
    /// </remarks>
    private async Task Advance(Event @event)
    {
        var correlationId = @event.Headers.CorrelationId;

        var pending = new[] { await LoadState(correlationId) }
            .OfType<InterceptionState>()
            .Where(state => TryAcceptEvent(state, @event));

        foreach (var state in pending)
            await Carry(correlationId, state, _process.React(state, @event));
    }

    /// <summary>
    /// Esegue una reazione: prima si mette al sicuro lo stato, poi si parla.
    /// </summary>
    /// <remarks>
    /// L'ordine conta. Un comando spedito prima del salvataggio puo' tornare indietro come evento
    /// mentre lo stato e' ancora quello vecchio, e il processo risponderebbe a se stesso sulla base
    /// di cose che non si e' ancora segnato.
    /// </remarks>
    private async Task Carry(Guid correlationId, InterceptionState state, InterceptionReaction reaction)
    {
        await SaveState(correlationId, state);

        foreach (var order in reaction.Orders)
            await SendCommand(order, correlationId);

        await (reaction.Outcome switch
        {
            InterceptionOutcome.Won => CompleteSaga(correlationId, state),
            InterceptionOutcome.Lost => FailSaga(correlationId, state, reaction.Reason),
            _ => Task.CompletedTask
        });
    }
}
