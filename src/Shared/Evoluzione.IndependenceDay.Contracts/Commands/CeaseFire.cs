namespace Evoluzione.IndependenceDay.Contracts.Commands;

public sealed class CeaseFire(EarthId aggregateId, ShipId shipId, Guid correlationId, Account who)
    : DefenseCommand(aggregateId, correlationId, who)
{
    public ShipId ShipId { get; } = shipId;
}
