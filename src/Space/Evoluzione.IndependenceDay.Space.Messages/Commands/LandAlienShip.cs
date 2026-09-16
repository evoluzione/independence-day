namespace Evoluzione.IndependenceDay.Space.Messages.Commands;

public sealed class LandAlienShip(ShipId aggregateId, CityId cityId, Guid correlationId, Account who)
    : SpaceCommand(aggregateId, correlationId, who)
{
    public ShipId ShipId { get; } = aggregateId;
    public CityId CityId { get; } = cityId;
}
