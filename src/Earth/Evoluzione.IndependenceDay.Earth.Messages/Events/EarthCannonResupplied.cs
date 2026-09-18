namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

public sealed class EarthCannonResupplied(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    int rounds,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
    public int Rounds { get; } = rounds;
}
