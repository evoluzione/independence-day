namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthCannonJammedEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthCannonJammed>(battle, feed, loggers)
{
    protected override async Task Project(EarthCannonJammed @event, CancellationToken ct)
    {
        await Battle.SetCannon(Id(@event.CityId), "jammed", Guid.Empty, @event.When(), ct);
        await Battle.Log(Id(@event.ShipId), "jammed", "cannone inceppato", Id(@event.CityId), "bad",
            @event.When(), ct);
    }
}
