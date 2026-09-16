namespace Evoluzione.IndependenceDay.Space.Messages.Events;

public sealed class AlienShipDestroyed(ShipId aggregateId, string cause, Guid correlationId)
    : DomainEvent(aggregateId, correlationId)
{
    public ShipId ShipId { get; } = aggregateId;
    public string Cause { get; } = cause;
}
