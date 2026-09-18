namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthCannonResuppliedEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthCannonResupplied>(battle, feed, loggers)
{
    protected override async Task Project(EarthCannonResupplied @event, CancellationToken ct)
    {
        await Battle.SetRounds(Id(@event.CityId), @event.Rounds, @event.When(), ct);
        await Battle.SetCannon(Id(@event.CityId), "ready", Guid.Empty, @event.When(), ct);
        await Battle.MarkResupply(Id(@event.CityId), Guid.Empty, @event.When(), ct);
        await Battle.Log(Id(@event.ShipId), "resupplied", "convoglio arrivato", Id(@event.CityId), "ok",
            @event.When(), ct);
    }
}
