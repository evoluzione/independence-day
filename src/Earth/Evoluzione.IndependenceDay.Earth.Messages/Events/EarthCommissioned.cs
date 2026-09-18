namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

public sealed class EarthCommissioned(EarthId aggregateId, int rounds, Guid correlationId)
    : DomainEvent(aggregateId, correlationId)
{
    public int Rounds { get; } = rounds;
}
