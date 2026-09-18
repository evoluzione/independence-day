namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthShotWastedEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthShotWasted>(battle, feed, loggers)
{
    protected override async Task Project(EarthShotWasted @event, CancellationToken ct)
    {
        await Battle.MarkShot(Id(@event.CityId), @event.RoundsLeft, @event.When(), ct);
        await Battle.Log(Id(@event.ShipId), "wasted", "colpo su un relitto", Id(@event.CityId), "bad",
            @event.When(), ct, 1);
    }
}
