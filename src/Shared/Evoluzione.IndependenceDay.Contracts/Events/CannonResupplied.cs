namespace Evoluzione.IndependenceDay.Contracts.Events;

public sealed class CannonResupplied(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    int rounds,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
    public int Rounds { get; } = rounds;
}
