namespace Evoluzione.IndependenceDay.Contracts.Events;

public sealed class NoCannonReady(EarthId aggregateId, ShipId shipId, Guid correlationId)
    : IntegrationEvent(aggregateId, correlationId)
{
    public ShipId ShipId { get; } = shipId;
}
