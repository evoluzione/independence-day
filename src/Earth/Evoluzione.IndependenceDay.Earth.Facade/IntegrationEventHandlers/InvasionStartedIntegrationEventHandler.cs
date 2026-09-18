using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.DomainIds;
using Evoluzione.IndependenceDay.Earth.ReadModel;
using Evoluzione.IndependenceDay.Infrastructure.Messaging;

namespace Evoluzione.IndependenceDay.Earth.Facade.IntegrationEventHandlers;

public class InvasionStartedIntegrationEventHandler(
    IServiceBus serviceBus,
    IBattleService battle,
    BattleFeed feed,
    ILoggerFactory loggerFactory) : IntegrationEventHandlerAsync<InvasionStarted>(loggerFactory)
{
    public override async Task HandleAsync(InvasionStarted @event,
        CancellationToken cancellationToken = default)
    {
        await battle.WaveStarted(@event.Wave, @event.Ships, @event.When(), cancellationToken);

        await serviceBus.SendAsync(
            new RecommissionEarth(EarthDefenses.Id, EarthDefenses.Rounds, @event.Wave,
                EarthDefenses.Integrity, @event.CorrelationId(), EarthDefenses.HighCommand),
            cancellationToken);

        await battle.LogInWave(Guid.Empty, "wave-start",
            $"{@event.Ships} navi in arrivo",
            Guid.Empty, "saga", @event.Wave, @event.When(), cancellationToken);

        feed.Notify();
    }
}
