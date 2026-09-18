namespace Evoluzione.IndependenceDay.Contracts.Events;

public sealed class ShipLanded(
    EarthId aggregateId,
    ShipId shipId,
    CityId cityId,
    int damage,
    int integrityLeft,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public ShipId ShipId { get; } = shipId;
    public CityId CityId { get; } = cityId;
    public int Damage { get; } = damage;
    public int IntegrityLeft { get; } = integrityLeft;
}
