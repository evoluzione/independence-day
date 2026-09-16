namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

/// <remarks>
/// La riparazione non ha preso: i colpi sono spesi e il cannone e' fermo com'era. Nel diario e' una
/// riga a se', perche' e' quella che spiega perche' le munizioni calano senza che nessuno spari.
/// </remarks>
public class EarthCannonStillJammedEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthCannonStillJammed>(battle, feed, loggers)
{
    protected override async Task Project(EarthCannonStillJammed @event, CancellationToken ct)
    {
        await Battle.SetRounds(Id(@event.CityId), @event.RoundsLeft, @event.When(), ct);
        await Battle.Log(Id(@event.ShipId), "still-jammed", "la riparazione non ha preso", Id(@event.CityId),
            "bad", @event.When(), ct);
    }
}
