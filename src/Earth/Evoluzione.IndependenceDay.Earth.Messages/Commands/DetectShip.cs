using Evoluzione.IndependenceDay.Contracts.World;

namespace Evoluzione.IndependenceDay.Earth.Messages.Commands;

public sealed class DetectShip(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    ShipClass shipClass,
    Guid correlationId,
    Account who) : Command(aggregateId, Guid.NewGuid(), who)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
    public ShipClass ShipClass { get; } = shipClass;
    public Guid CorrelationId { get; } = correlationId;
}
