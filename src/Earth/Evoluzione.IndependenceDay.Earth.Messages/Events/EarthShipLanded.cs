namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

public sealed class EarthShipLanded(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    int damage,
    int integrityLeft,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
    public int Damage { get; } = damage;
    public int IntegrityLeft { get; } = integrityLeft;
}
