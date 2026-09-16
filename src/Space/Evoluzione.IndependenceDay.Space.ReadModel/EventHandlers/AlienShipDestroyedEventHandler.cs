namespace Evoluzione.IndependenceDay.Space.ReadModel.EventHandlers;

public class AlienShipDestroyedEventHandler(IShipsService service, ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<AlienShipDestroyed>(loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<AlienShipDestroyedEventHandler>();

    public override async Task HandleAsync(AlienShipDestroyed @event, CancellationToken cancellationToken = default)
    {
        try
        {
            await service.Close(Guid.Parse(@event.ShipId.Value), "destroyed", @event.Cause, @event.When(),
                @event.EventRevision(), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[AlienShipDestroyedEventHandler] Read model non aggiornato per la nave {ShipId}",
                @event.ShipId.Value);
        }
    }
}
