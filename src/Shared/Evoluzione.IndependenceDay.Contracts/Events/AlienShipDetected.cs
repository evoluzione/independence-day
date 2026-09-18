using Evoluzione.IndependenceDay.Contracts.World;

namespace Evoluzione.IndependenceDay.Contracts.Events;

public sealed class AlienShipDetected(
    ShipId aggregateId,
    CityId targetCity,
    ShipClass shipClass,
    int wave,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public ShipId ShipId { get; } = aggregateId;
    public CityId TargetCity { get; } = targetCity;
    public ShipClass ShipClass { get; } = shipClass;
    public int Wave { get; } = wave;
}
