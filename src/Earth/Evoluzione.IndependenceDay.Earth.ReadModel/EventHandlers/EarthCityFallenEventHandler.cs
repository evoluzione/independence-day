namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public class EarthCityFallenEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthCityFallen>(battle, feed, loggers)
{
    protected override async Task Project(EarthCityFallen @event, CancellationToken ct)
    {
        await Battle.SetCannon(Id(@event.CityId), "lost", Guid.Empty, @event.When(), ct);
        await Battle.Log(Id(@event.ShipId), "city-fallen", "citta' rasa al suolo, cannone perduto",
            Id(@event.CityId), "bad", @event.When(), ct);
    }
}
