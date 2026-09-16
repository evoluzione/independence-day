using Muflone;
using C = Evoluzione.IndependenceDay.Contracts.Events;
using ContractsInvasionId = Evoluzione.IndependenceDay.Contracts.Ids.InvasionId;

namespace Evoluzione.IndependenceDay.Space.ReadModel.EventHandlers;

public class InvasionEndedPublisher(IEventBus bus, ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<InvasionWaveEnded>(loggerFactory)
{
    public override Task HandleAsync(InvasionWaveEnded @event, CancellationToken ct = default) =>
        bus.PublishAsync(new C.InvasionEnded(
            new ContractsInvasionId(Guid.Parse(@event.InvasionId.Value)), @event.Wave, @event.Level,
            @event.CorrelationId()), ct);
}
