namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthShotMissedEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthShotMissed>(battle, feed, loggers)
{
    protected override async Task Project(EarthShotMissed @event, CancellationToken ct)
    {
        await Battle.MarkShot(Id(@event.CityId), @event.RoundsLeft, @event.When(), ct);
        await Battle.Log(Id(@event.ShipId), "missed", $"bersaglio mancato, {@event.RoundsLeft} in canna",
            Id(@event.CityId), "warn", @event.When(), ct, 1);
    }
}
