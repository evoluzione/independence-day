namespace Evoluzione.IndependenceDay.Earth.Messages.Commands;

public sealed class CommissionEarth(
    EarthId aggregateId,
    int rounds,
    int integrity,
    Guid correlationId,
    Account who) : Command(aggregateId, Guid.NewGuid(), who)
{
    public int Rounds { get; } = rounds;

    public int Integrity { get; } = integrity;
    public Guid CorrelationId { get; } = correlationId;
}
