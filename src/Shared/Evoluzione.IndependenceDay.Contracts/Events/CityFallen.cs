namespace Evoluzione.IndependenceDay.Contracts.Events;

public sealed class CityFallen(EarthId aggregateId, CityId cityId, ShipId shipId, Guid correlationId)
    : IntegrationEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
}
