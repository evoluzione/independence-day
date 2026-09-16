namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

/// <summary>La riparazione non ha preso: il cannone e' ancora inceppato, e i colpi sono andati.</summary>
public sealed class EarthCannonStillJammed(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    int roundsLeft,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
    public int RoundsLeft { get; } = roundsLeft;
}
