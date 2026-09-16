namespace Evoluzione.IndependenceDay.Space.Messages.Commands;

public sealed class DestroyAlienShip(ShipId aggregateId, string cause, Guid correlationId, Account who)
    : SpaceCommand(aggregateId, correlationId, who)
{
    public ShipId ShipId { get; } = aggregateId;
    public string Cause { get; } = cause;
}
