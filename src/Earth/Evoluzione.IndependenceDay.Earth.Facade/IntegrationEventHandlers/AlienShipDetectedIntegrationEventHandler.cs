using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.DomainIds;
using Evoluzione.IndependenceDay.Infrastructure.Messaging;

namespace Evoluzione.IndependenceDay.Earth.Facade.IntegrationEventHandlers;

/// <summary>
/// L'avvistamento dello Spazio diventa una presa in carico della Terra.
/// </summary>
/// <remarks>
/// Il quadro della Terra non si scrive qui: lo scrive la proiezione di <c>EarthShipDetected</c>,
/// cioe' dopo che l'aggregato ha accettato la nave. Scriverlo prima vorrebbe dire mostrare navi che
/// la difesa non ha mai preso in carico, e accettare ordini su di loro.
/// </remarks>
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
