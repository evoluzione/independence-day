using Evoluzione.IndependenceDay.Contracts.World;

namespace Evoluzione.IndependenceDay.Space.Messages.Events;

public sealed class AlienShipLaunched(
    ShipId aggregateId,
    CityId targetCity,
    MotherShipId motherShip,
    ShipClass shipClass,
    int wave,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public ShipId ShipId { get; } = aggregateId;
    public CityId TargetCity { get; } = targetCity;
    public MotherShipId MotherShip { get; } = motherShip;
    public ShipClass ShipClass { get; } = shipClass;
    public int Wave { get; } = wave;
}
