namespace Evoluzione.IndependenceDay.Contracts.Events;

/// <summary>
/// Il cannone ha smesso di sparare ed e' di nuovo disponibile.
/// </summary>
/// <remarks>
/// E' la <b>conferma</b> della compensazione, e va aspettata. Anche un cessate il fuoco puo'
/// perdersi per strada: chi lo manda e poi chiude senza aspettare questa riga lascia un cannone a
/// sparare su relitti e non lo sapra' mai.
/// </remarks>
public sealed class FireCeased(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    int roundsLeft,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
    public int RoundsLeft { get; } = roundsLeft;
}
