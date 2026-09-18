namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthCannonEmptyEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthCannonEmpty>(battle, feed, loggers)
{
    protected override async Task Project(EarthCannonEmpty @event, CancellationToken ct)
    {
        await Battle.SetRounds(Id(@event.CityId), 0, @event.When(), ct);
        await Battle.SetCannon(Id(@event.CityId), "empty", Guid.Empty, @event.When(), ct);
        await Battle.Log(Id(@event.ShipId), "empty", "munizioni finite", Id(@event.CityId), "bad",
            @event.When(), ct);
    }
}
