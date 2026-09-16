namespace Evoluzione.IndependenceDay.Space.ReadModel.EventHandlers;

/// <summary>Campagna nuova: tutte le citta' tornano bersagli validi.</summary>
public class CitiesResetOnNewCampaignEventHandler(ITargetCityService cities, ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<InvasionWaveStarted>(loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<CitiesResetOnNewCampaignEventHandler>();

    public override async Task HandleAsync(InvasionWaveStarted @event, CancellationToken cancellationToken = default)
    {
        if (@event.Level != 1)
            return;

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
