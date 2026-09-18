using Muflone;
using ContractsShipId = Evoluzione.IndependenceDay.Contracts.Ids.ShipId;
using ContractsCityId = Evoluzione.IndependenceDay.Contracts.Ids.CityId;

namespace Evoluzione.IndependenceDay.Space.ReadModel.EventHandlers;

public class AlienShipDetectedPublisher(IEventBus eventBus, ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<AlienShipLaunched>(loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<AlienShipDetectedPublisher>();

    public override async Task HandleAsync(AlienShipLaunched @event, CancellationToken cancellationToken = default)
    {
        var detected = new Contracts.Events.AlienShipDetected(
            new ContractsShipId(Guid.Parse(@event.ShipId.Value)),
            new ContractsCityId(Guid.Parse(@event.TargetCity.Value)),
            @event.ShipClass,
            @event.Wave,
            @event.CorrelationId());

        _logger.LogInformation("[Space] {Class} {ShipId} in rotta sulla citta' {CityId}",
            Contracts.World.Ships.NameOf(@event.ShipClass), @event.ShipId.Value, @event.TargetCity.Value);

        await eventBus.PublishAsync(detected, cancellationToken);
    }
}
