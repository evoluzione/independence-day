using Muflone;
using C = Evoluzione.IndependenceDay.Contracts.Events;
using ContractsInvasionId = Evoluzione.IndependenceDay.Contracts.Ids.InvasionId;

namespace Evoluzione.IndependenceDay.Space.ReadModel.EventHandlers;

public class InvasionWaveEndedEventHandler(IInvasionProgressService service, ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<InvasionWaveEnded>(loggerFactory)

{
    private readonly ILogger _logger = loggerFactory.CreateLogger<InvasionWaveEndedEventHandler>();

    public override async Task HandleAsync(InvasionWaveEnded @event, CancellationToken cancellationToken = default)
    {
        try
        {
            await service.WaveEnded(Guid.Parse(@event.InvasionId.Value), @event.When(), @event.EventRevision(),
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Space] Read model non aggiornato per l'ondata {Wave}", @event.Wave);
        }
    }
}
