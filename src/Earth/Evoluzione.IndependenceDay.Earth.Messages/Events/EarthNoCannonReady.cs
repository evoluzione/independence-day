namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

public sealed class EarthNoCannonReady(EarthId aggregateId, ShipId shipId, Guid correlationId)
    : DomainEvent(aggregateId, correlationId)
{
    public ShipId ShipId { get; } = shipId;
}
