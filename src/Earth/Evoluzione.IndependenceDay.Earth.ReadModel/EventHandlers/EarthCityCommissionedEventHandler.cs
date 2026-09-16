namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthCityCommissionedEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthCityCommissioned>(battle, feed, loggers)
{
    protected override Task Project(EarthCityCommissioned @event, CancellationToken ct) =>
        Battle.CommissionCity(Id(@event.CityId), @event.Name, @event.Integrity, @event.Rounds, @event.When(),
            @event.EventRevision(), ct);
}
