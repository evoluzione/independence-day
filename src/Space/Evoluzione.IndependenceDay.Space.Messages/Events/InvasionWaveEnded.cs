namespace Evoluzione.IndependenceDay.Space.Messages.Events;

public sealed class InvasionWaveEnded(InvasionId aggregateId, int wave, int level, Guid correlationId)
    : DomainEvent(aggregateId, correlationId)
{
    public InvasionId InvasionId { get; } = aggregateId;
    public int Wave { get; } = wave;
    public int Level { get; } = level;
}
