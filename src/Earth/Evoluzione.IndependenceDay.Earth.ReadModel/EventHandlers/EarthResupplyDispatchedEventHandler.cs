namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthResupplyDispatchedEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthResupplyDispatched>(battle, feed, loggers)
{
    protected override async Task Project(EarthResupplyDispatched @event, CancellationToken ct)
    {
        await Battle.SetCannonStatus(Id(@event.CityId), "resupplying", @event.When(), ct);
        await Battle.MarkResupply(Id(@event.CityId), Id(@event.ShipId), @event.When(), ct);
        await Battle.Log(Id(@event.ShipId), "resupply-dispatched", "convoglio chiamato", Id(@event.CityId), "bad",
            @event.When(), ct);
    }
}
