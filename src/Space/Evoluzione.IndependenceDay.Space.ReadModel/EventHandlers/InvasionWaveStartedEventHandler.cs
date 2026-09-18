using Muflone;
using C = Evoluzione.IndependenceDay.Contracts.Events;
using ContractsInvasionId = Evoluzione.IndependenceDay.Contracts.Ids.InvasionId;

namespace Evoluzione.IndependenceDay.Space.ReadModel.EventHandlers;

public class InvasionWaveStartedEventHandler(IInvasionProgressService service, ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<InvasionWaveStarted>(loggerFactory)

{
    private readonly ILogger _logger = loggerFactory.CreateLogger<InvasionWaveStartedEventHandler>();

    public override async Task HandleAsync(InvasionWaveStarted @event, CancellationToken cancellationToken = default)
    {
        try
        {
            await service.WaveStarted(Guid.Parse(@event.InvasionId.Value), @event.Wave, @event.Level,
                @event.Ships, @event.When(), @event.EventRevision(), cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Space] Read model non aggiornato per l'ondata {Wave}", @event.Wave);
        }
    }
}
