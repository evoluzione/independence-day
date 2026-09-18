namespace Evoluzione.IndependenceDay.Contracts.Commands;

public sealed class OpenFire(EarthId aggregateId, ShipId shipId, Guid correlationId, Account who)
    : DefenseCommand(aggregateId, correlationId, who)
{
    public ShipId ShipId { get; } = shipId;
}
