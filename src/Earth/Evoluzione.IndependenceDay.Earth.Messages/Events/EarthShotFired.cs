namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

public sealed class EarthShotFired(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    int hits,
    int roundsLeft,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;

    public int Hits { get; } = hits;

    public int RoundsLeft { get; } = roundsLeft;
}
