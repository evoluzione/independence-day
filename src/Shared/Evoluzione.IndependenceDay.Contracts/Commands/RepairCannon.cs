namespace Evoluzione.IndependenceDay.Contracts.Commands;

public sealed class RepairCannon(EarthId aggregateId, CityId cityId, ShipId shipId, Guid correlationId, Account who)
    : DefenseCommand(aggregateId, correlationId, who)
{
    public CityId CityId { get; } = cityId;

    public ShipId ShipId { get; } = shipId;
}
