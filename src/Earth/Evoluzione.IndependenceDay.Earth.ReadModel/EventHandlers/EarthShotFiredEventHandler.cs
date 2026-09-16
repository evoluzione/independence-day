namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthShotFiredEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthShotFired>(battle, feed, loggers)
{
    protected override async Task Project(EarthShotFired @event, CancellationToken ct)
    {
        await Battle.MarkShot(Id(@event.CityId), @event.RoundsLeft, @event.When(), ct);
        await Battle.SetHits(Id(@event.ShipId), @event.Hits, @event.When(), ct);
        await Battle.Log(Id(@event.ShipId), "shot", $"colpo a segno, {@event.RoundsLeft} in canna",
            Id(@event.CityId), "ok", @event.When(), ct, 1);
    }
}
