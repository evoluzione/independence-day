using Evoluzione.IndependenceDay.Contracts.World;

namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthShipDetectedEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthShipDetected>(battle, feed, loggers)
{
    protected override async Task Project(EarthShipDetected @event, CancellationToken ct)
    {
        var wave = (await Battle.State(ct))?.Wave ?? 0;

        await Battle.DetectShip(Id(@event.ShipId), Id(@event.CityId), @event.ShipClass, wave,
            @event.CorrelationId(), @event.When(), @event.EventRevision(), ct);

        await Battle.LogInWave(Id(@event.ShipId), "detected",
            $"{Ships.NameOf(@event.ShipClass)} in avvicinamento", Id(@event.CityId), "warn", wave,
            @event.When(), ct);
    }
}
