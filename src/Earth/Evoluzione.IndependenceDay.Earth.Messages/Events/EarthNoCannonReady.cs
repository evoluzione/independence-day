namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

/// <summary>Nessun cannone libero per quella nave: tutti impegnati, inceppati o a secco.</summary>
public sealed class EarthNoCannonReady(EarthId aggregateId, ShipId shipId, Guid correlationId)
    : DomainEvent(aggregateId, correlationId)
{
    public ShipId ShipId { get; } = shipId;
}
