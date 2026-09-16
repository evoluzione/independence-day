using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Sagas.Infrastructure.MongoDB;
using Microsoft.Extensions.Logging;
using Muflone.Messages.Events;

namespace Evoluzione.IndependenceDay.Sagas.Facade.IntegrationEventHandlers;

/// <summary>
/// Una campagna nuova: la sala operativa torna vuota.
/// </summary>
/// <remarks>
/// Le linee sono finite, e un processo le libera solo chiudendosi. Quelli rimasti aperti da una
/// partita persa non si chiuderanno mai piu': nessuno mandera' altri eventi a quelle navi, perche'
/// quelle navi non esistono piu'.
/// <para>
/// Senza questo giro le linee non tornerebbero indietro fra una partita e l'altra, e dopo due o tre
/// tentativi il gioco sarebbe murato: le navi vengono avvistate, nessuno le prende in carico, e le
/// citta' cadono tutte alla prima ondata senza un solo ordine spedito. Che e' esattamente il modo in
/// cui si presenta una linea occupata — in silenzio.
/// </para>
/// <para>
/// Solo al livello uno, come per le citta': fra un livello e l'altro non si ripristina niente.
/// </para>
/// </remarks>
public class InvasionStartedIntegrationEventHandler(
    IOperationsRoom operationsRoom,
    ILoggerFactory loggerFactory) : IntegrationEventHandlerAsync<InvasionStarted>(loggerFactory)
{
    public override Task HandleAsync(InvasionStarted @event, CancellationToken cancellationToken = default) =>
        @event.Level == 1 ? operationsRoom.FreeLines(cancellationToken) : Task.CompletedTask;
}
