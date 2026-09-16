namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthNoCannonReadyEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthNoCannonReady>(battle, feed, loggers)
{
    protected override Task Project(EarthNoCannonReady @event, CancellationToken ct) =>
        Battle.Log(Id(@event.ShipId), "no-cannon", "nessun cannone disponibile", Guid.Empty, "warn",
            @event.When(), ct);
}
