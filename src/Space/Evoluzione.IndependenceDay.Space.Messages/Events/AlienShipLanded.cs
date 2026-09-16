namespace Evoluzione.IndependenceDay.Space.Messages.Events;

public sealed class AlienShipLanded(ShipId aggregateId, CityId cityId, Guid correlationId)
    : DomainEvent(aggregateId, correlationId)
{
    public ShipId ShipId { get; } = aggregateId;
    public CityId CityId { get; } = cityId;
}
