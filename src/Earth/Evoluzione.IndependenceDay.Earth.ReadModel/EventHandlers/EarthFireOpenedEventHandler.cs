namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthFireOpenedEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthFireOpened>(battle, feed, loggers)
{
    protected override async Task Project(EarthFireOpened @event, CancellationToken ct)
    {
        await Battle.SetCannon(Id(@event.CityId), "firing", Id(@event.ShipId), @event.When(), ct);
        await Battle.Log(Id(@event.ShipId), "fire", "fuoco aperto", Id(@event.CityId), "ok", @event.When(), ct);
    }
}
