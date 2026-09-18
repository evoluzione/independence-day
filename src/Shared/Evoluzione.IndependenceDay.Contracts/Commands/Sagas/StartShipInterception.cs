namespace Evoluzione.IndependenceDay.Contracts.Commands.Sagas;

public sealed class StartShipInterception(
    ShipId aggregateId,
    CityId targetCity,
    Guid correlationId,
    Account who) : DefenseCommand(aggregateId, correlationId, who)
{
    public ShipId ShipId { get; } = aggregateId;
    public CityId TargetCity { get; } = targetCity;
}
