using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Infrastructure.Messaging;

namespace Evoluzione.IndependenceDay.Space.Facade.IntegrationEventHandlers;

public class ShipLandedIntegrationEventHandler(IServiceBus serviceBus, ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<ShipLanded>(loggerFactory)


{
    public override Task HandleAsync(ShipLanded @event, CancellationToken cancellationToken = default) =>
        serviceBus.SendAsync(
            new LandAlienShip(new ShipId(Guid.Parse(@event.ShipId.Value)),
                new CityId(Guid.Parse(@event.CityId.Value)), @event.CorrelationId(), SpaceAccounts.Earth),
            cancellationToken);
}
