namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

/// <summary>
/// L'ordine perso finisce nel diario e basta.
/// </summary>
/// <remarks>
/// Non esiste un publisher per questo evento, ed e' voluto: chi ha ordinato non deve ricevere niente.
/// La riga a schermo serve solo a chi, dopo, si chiede perche' quella citta' era scoperta.
/// </remarks>
public class EarthOrderLostEventHandler(IBattleService battle, BattleFeed feed, ILoggerFactory loggers)
    : EarthProjection<EarthOrderLost>(battle, feed, loggers)
{
    protected override Task Project(EarthOrderLost @event, CancellationToken ct) =>
        Battle.Log(Id(@event.ShipId), "order-lost", $"ordine perso: {@event.Order}", Id(@event.CityId), "warn",
            @event.When(), ct);
}
