using Evoluzione.IndependenceDay.Contracts.Commands.Sagas;
using Evoluzione.IndependenceDay.Contracts.Events;
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
/// </remarks>
public class ShipDetectedIntegrationEventHandler(IServiceBus serviceBus, ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<ShipDetected>(loggerFactory)
{
    private static readonly Account Coordinator = new("saga", "Ship Interception");

    public override Task HandleAsync(ShipDetected @event, CancellationToken cancellationToken = default) =>
        serviceBus.SendAsync(
            new StartShipInterception(@event.ShipId, @event.CityId, @event.Headers.CorrelationId, Coordinator),
            cancellationToken);
}
