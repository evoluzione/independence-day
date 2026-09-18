namespace Evoluzione.IndependenceDay.Space.ReadModel.EventHandlers;

public class CitiesResetOnNewCampaignEventHandler(ITargetCityService cities, ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<InvasionWaveStarted>(loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<CitiesResetOnNewCampaignEventHandler>();

    public override async Task HandleAsync(InvasionWaveStarted @event, CancellationToken cancellationToken = default)
    {
        try
        {
            await cities.ResetAll(@event.When(), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Space] Bersagli non azzerati per l'ondata {Wave}", @event.Wave);
        }
    }
}
