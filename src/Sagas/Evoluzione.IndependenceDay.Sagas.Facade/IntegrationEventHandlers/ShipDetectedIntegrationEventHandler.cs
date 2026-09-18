using Evoluzione.IndependenceDay.Contracts.Commands.Sagas;
using Evoluzione.IndependenceDay.Contracts.Events;
using Microsoft.Extensions.Logging;
using Muflone.CustomTypes;
using Muflone.Messages.Events;
using Muflone.Persistence;

namespace Evoluzione.IndependenceDay.Sagas.Facade.IntegrationEventHandlers;

public class ShipDetectedIntegrationEventHandler(
    IServiceBus serviceBus,
    ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<ShipDetected>(loggerFactory)
{
    private static readonly Account Coordinator = new("saga", "Ship Interception");

    public override Task HandleAsync(ShipDetected @event, CancellationToken cancellationToken = default) =>
        serviceBus.SendAsync(
            new StartShipInterception(@event.ShipId, @event.CityId, @event.Headers.CorrelationId, Coordinator),
            cancellationToken);
}
