namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthRecommissionedEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthRecommissioned>(battle, feed, loggers)
{
    protected override Task Project(EarthRecommissioned @event, CancellationToken ct) =>
        Battle.ResetCities(@event.Integrity, @event.Rounds, @event.When(), ct);
}
