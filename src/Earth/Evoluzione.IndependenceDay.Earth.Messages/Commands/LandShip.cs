namespace Evoluzione.IndependenceDay.Earth.Messages.Commands;

/// <summary>Il tempo e' scaduto: la nave tocca terra.</summary>
public sealed class LandShip(
    EarthId aggregateId,
    ShipId shipId,
    Guid correlationId,
    Account who) : Command(aggregateId, Guid.NewGuid(), who)
{
    public ShipId ShipId { get; } = shipId;
    public Guid CorrelationId { get; } = correlationId;
}
