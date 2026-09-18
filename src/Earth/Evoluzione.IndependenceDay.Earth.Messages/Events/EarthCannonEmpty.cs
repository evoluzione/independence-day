namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

public sealed class EarthCannonEmpty(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
}
