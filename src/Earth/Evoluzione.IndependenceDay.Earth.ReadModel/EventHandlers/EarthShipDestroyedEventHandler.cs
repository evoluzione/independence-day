namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthShipDestroyedEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthShipDestroyed>(battle, feed, loggers)
{
    protected override async Task Project(EarthShipDestroyed @event, CancellationToken ct)
    {
        await Battle.CloseShip(Id(@event.ShipId), "destroyed", @event.When(), ct);
        await Battle.Log(Id(@event.ShipId), "destroyed", "nave abbattuta", Id(@event.CityId), "ok",
            @event.When(), ct);
    }
}
