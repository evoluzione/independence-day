using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.DomainIds;
using Evoluzione.IndependenceDay.Infrastructure.Messaging;

namespace Evoluzione.IndependenceDay.Earth.Facade.IntegrationEventHandlers;

public class AlienShipDetectedIntegrationEventHandler(
    IServiceBus serviceBus,
    ILoggerFactory loggerFactory) : IntegrationEventHandlerAsync<AlienShipDetected>(loggerFactory)
{
    public override Task HandleAsync(AlienShipDetected @event, CancellationToken cancellationToken = default) =>
        serviceBus.SendAsync(
            new DetectShip(EarthDefenses.Id,
                new CityId(Guid.Parse(@event.TargetCity.Value)),
                new ShipId(Guid.Parse(@event.ShipId.Value)),
                @event.ShipClass,
                @event.CorrelationId(),
                EarthDefenses.HighCommand),
            cancellationToken);
}
