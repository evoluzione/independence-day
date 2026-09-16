using Evoluzione.IndependenceDay.Contracts.Commands.Sagas;
using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Sagas.Infrastructure.MongoDB;
using Microsoft.Extensions.Logging;
using Muflone.CustomTypes;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace Evoluzione.IndependenceDay.Sagas.Facade.IntegrationEventHandlers;

/// <summary>
/// La presa in carico accende il processo.
/// </summary>
/// <remarks>
/// Un processo si avvia con un comando, non con un evento: questo handler e' il solo pezzo che
/// traduce "e' successo qualcosa" in "fai qualcosa". Parte dalla presa in carico e non
/// dall'avvistamento dello Spazio, perche' solo da li' in poi la Terra accetta ordini di fuoco su
/// quella nave.
/// <para>
/// La sala operativa ha un numero finito di linee, e questo e' il punto in cui si contano. Se sono
/// tutte occupate la nave <b>non viene presa in carico</b>: nessun ordine, nessun cannone, nessun
/// errore. Le linee non si liberano da sole — le libera un processo che si chiude.
/// </para>
/// </remarks>
public class ShipDetectedIntegrationEventHandler(
    IServiceBus serviceBus,
    IOperationsRoom operationsRoom,
    ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<ShipDetected>(loggerFactory)
{
    private static readonly Account Coordinator = new("saga", "Ship Interception");

    public override async Task HandleAsync(ShipDetected @event, CancellationToken cancellationToken = default)
    {
        if (await operationsRoom.OpenLines(cancellationToken) >= OperationsRoom.Lines)
            return;

        await serviceBus.SendAsync(
            new StartShipInterception(@event.ShipId, @event.CityId, @event.Headers.CorrelationId, Coordinator),
            cancellationToken);
    }
}
