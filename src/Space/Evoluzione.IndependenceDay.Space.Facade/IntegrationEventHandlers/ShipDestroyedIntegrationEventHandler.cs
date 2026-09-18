using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Infrastructure.Messaging;

namespace Evoluzione.IndependenceDay.Space.Facade.IntegrationEventHandlers;

public class ShipDestroyedIntegrationEventHandler(IServiceBus serviceBus, ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<ShipDestroyed>(loggerFactory)

{
    public override Task HandleAsync(ShipDestroyed @event, CancellationToken cancellationToken = default) =>
        serviceBus.SendAsync(
            new DestroyAlienShip(new ShipId(Guid.Parse(@event.ShipId.Value)), "abbattuta",
                @event.CorrelationId(), SpaceAccounts.Earth),
            cancellationToken);
}
