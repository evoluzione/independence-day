namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthShipLandedEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthShipLanded>(battle, feed, loggers)
{
    protected override async Task Project(EarthShipLanded @event, CancellationToken ct)
    {
        await Battle.CloseShip(Id(@event.ShipId), "landed", @event.When(), ct);
        await Battle.SetIntegrity(Id(@event.CityId), @event.IntegrityLeft, @event.When(),
            @event.EventRevision(), ct);
        await Battle.Log(Id(@event.ShipId), "landed",
            $"nave a terra: {@event.Damage} di danno, integrita' {@event.IntegrityLeft}", Id(@event.CityId),
            "bad", @event.When(), ct);
    }
}
