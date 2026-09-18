namespace Evoluzione.IndependenceDay.Space.ReadModel.EventHandlers;

public class AlienShipLaunchedEventHandler(IShipsService service, ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<AlienShipLaunched>(loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<AlienShipLaunchedEventHandler>();

    public override async Task HandleAsync(AlienShipLaunched @event, CancellationToken cancellationToken = default)
    {
        try
        {
            await service.Launch(
                Guid.Parse(@event.ShipId.Value),
                Guid.Parse(@event.TargetCity.Value),
                Guid.Parse(@event.MotherShip.Value),
                @event.ShipClass,
                @event.Wave,
                @event.When(),
                @event.EventRevision(),
                cancellationToken);
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, "[AlienShipLaunchedEventHandler] Read model non aggiornato per la nave {ShipId}",
                @event.ShipId.Value);
        }
    }
}
