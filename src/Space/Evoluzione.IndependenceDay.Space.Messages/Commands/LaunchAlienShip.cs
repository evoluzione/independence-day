using Evoluzione.IndependenceDay.Contracts.World;

namespace Evoluzione.IndependenceDay.Space.Messages.Commands;

public sealed class LaunchAlienShip(
    ShipId aggregateId,
    CityId targetCity,
    MotherShipId motherShip,
    ShipClass shipClass,
    int wave,
    Guid correlationId,
    Account who) : SpaceCommand(aggregateId, correlationId, who)
{
    public ShipId ShipId { get; } = aggregateId;
    public CityId TargetCity { get; } = targetCity;
    public MotherShipId MotherShip { get; } = motherShip;
    public ShipClass ShipClass { get; } = shipClass;
    public int Wave { get; } = wave;
}
