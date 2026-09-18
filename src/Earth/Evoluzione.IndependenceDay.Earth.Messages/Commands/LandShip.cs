namespace Evoluzione.IndependenceDay.Earth.Messages.Commands;

public sealed class LandShip(
    EarthId aggregateId,
    ShipId shipId,
    Guid correlationId,
    Account who) : Command(aggregateId, Guid.NewGuid(), who)
{
    public ShipId ShipId { get; } = shipId;
    public Guid CorrelationId { get; } = correlationId;
}
