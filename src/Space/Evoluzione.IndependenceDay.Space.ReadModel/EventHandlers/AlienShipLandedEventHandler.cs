namespace Evoluzione.IndependenceDay.Space.ReadModel.EventHandlers;

public class AlienShipLandedEventHandler(IShipsService service, ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<AlienShipLanded>(loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<AlienShipLandedEventHandler>();

    public override async Task HandleAsync(AlienShipLanded @event, CancellationToken cancellationToken = default)
    {
        try
        {
            await service.Close(Guid.Parse(@event.ShipId.Value), "landed", "landed", @event.When(),
                @event.EventRevision(), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AlienShipLandedEventHandler] Read model non aggiornato per la nave {ShipId}",
                @event.ShipId.Value);
        }
    }
}
