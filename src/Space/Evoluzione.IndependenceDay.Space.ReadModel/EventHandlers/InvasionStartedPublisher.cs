using Muflone;
using C = Evoluzione.IndependenceDay.Contracts.Events;
using ContractsInvasionId = Evoluzione.IndependenceDay.Contracts.Ids.InvasionId;

namespace Evoluzione.IndependenceDay.Space.ReadModel.EventHandlers;

public class InvasionStartedPublisher(IEventBus bus, ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<InvasionWaveStarted>(loggerFactory)

{
    public override Task HandleAsync(InvasionWaveStarted @event, CancellationToken ct = default) =>
        bus.PublishAsync(new C.InvasionStarted(
            new ContractsInvasionId(Guid.Parse(@event.InvasionId.Value)), @event.Wave, @event.Level,
            @event.Ships, @event.CorrelationId()), ct);
}
