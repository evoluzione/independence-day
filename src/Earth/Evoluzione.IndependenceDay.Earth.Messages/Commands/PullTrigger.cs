namespace Evoluzione.IndependenceDay.Earth.Messages.Commands;

public sealed class PullTrigger(
    EarthId aggregateId,
    CityId cityId,
    Guid correlationId,
    Account who) : Command(aggregateId, Guid.NewGuid(), who)
{
    public CityId CityId { get; } = cityId;
    public Guid CorrelationId { get; } = correlationId;
}
