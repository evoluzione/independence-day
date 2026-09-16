namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthCannonEmptyEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthCannonEmpty>(battle, feed, loggers)
{
    protected override async Task Project(EarthCannonEmpty @event, CancellationToken ct)
    {
        await Battle.SetRounds(Id(@event.CityId), 0, @event.When(), ct);
        // Il bersaglio resta: a secco o no, quel cannone e' ancora impegnato su quella nave.
        await Battle.SetCannonStatus(Id(@event.CityId), "empty", @event.When(), ct);
        await Battle.SetJammedOn(Id(@event.CityId), Guid.Empty, @event.When(), ct);
        await Battle.Log(Id(@event.ShipId), "empty", "munizioni finite", Id(@event.CityId), "bad",
            @event.When(), ct);
    }
}
