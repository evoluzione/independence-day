namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthCannonRepairedEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthCannonRepaired>(battle, feed, loggers)
{
    protected override async Task Project(EarthCannonRepaired @event, CancellationToken ct)
    {
        await Battle.SetRounds(Id(@event.CityId), @event.RoundsLeft, @event.When(), ct);
        await Battle.SetCannon(Id(@event.CityId), "ready", Guid.Empty, @event.When(), ct);
        await Battle.SetJammedOn(Id(@event.CityId), Guid.Empty, @event.When(), ct);
        await Battle.Log(Id(@event.ShipId), "repaired", "cannone riparato", Id(@event.CityId), "ok",
            @event.When(), ct);
    }
}
