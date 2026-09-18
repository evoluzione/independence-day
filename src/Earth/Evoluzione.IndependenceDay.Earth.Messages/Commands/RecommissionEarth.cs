namespace Evoluzione.IndependenceDay.Earth.Messages.Commands;

public sealed class RecommissionEarth(
    EarthId aggregateId,
    int rounds,
    int wave,
    int integrity,
    Guid correlationId,
    Account who) : Command(aggregateId, Guid.NewGuid(), who)
{
    public int Rounds { get; } = rounds;
    public int Wave { get; } = wave;
    public int Integrity { get; } = integrity;
    public Guid CorrelationId { get; } = correlationId;
}
