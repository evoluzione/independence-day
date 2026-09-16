using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Earth.ReadModel;
using Evoluzione.IndependenceDay.Infrastructure.Messaging;

namespace Evoluzione.IndependenceDay.Earth.Facade.IntegrationEventHandlers;

public class InvasionEndedIntegrationEventHandler(
    IBattleService battle,
    BattleFeed feed,
    ILoggerFactory loggerFactory) : IntegrationEventHandlerAsync<InvasionEnded>(loggerFactory)
{
    public override async Task HandleAsync(InvasionEnded @event, CancellationToken cancellationToken = default)
    {
        await battle.WaveEnded(@event.Wave, @event.When(), cancellationToken);

        feed.Notify();
    }
}
