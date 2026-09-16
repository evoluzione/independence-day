namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthFireCeasedEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthFireCeased>(battle, feed, loggers)
{
    protected override async Task Project(EarthFireCeased @event, CancellationToken ct)
    {
        await Battle.SetCannon(Id(@event.CityId), @event.RoundsLeft > 0 ? "ready" : "empty", Guid.Empty,
            @event.When(), ct);
        await Battle.Log(Id(@event.ShipId), "cease", "cannone di nuovo disponibile", Id(@event.CityId), "ok",
            @event.When(), ct);
    }
}
