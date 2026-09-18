namespace Evoluzione.IndependenceDay.Earth.Messages.Commands;

public sealed class DeliverSupplies(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    Guid correlationId,
    Account who) : Command(aggregateId, Guid.NewGuid(), who)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
    public Guid CorrelationId { get; } = correlationId;
}
