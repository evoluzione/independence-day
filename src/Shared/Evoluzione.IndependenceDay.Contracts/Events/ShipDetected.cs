namespace Evoluzione.IndependenceDay.Contracts.Events;

public sealed class ShipDetected(
    EarthId aggregateId,
    ShipId shipId,
    CityId cityId,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public ShipId ShipId { get; } = shipId;

    public CityId CityId { get; } = cityId;
}
