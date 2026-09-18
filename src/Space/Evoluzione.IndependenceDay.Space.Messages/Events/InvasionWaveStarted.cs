namespace Evoluzione.IndependenceDay.Space.Messages.Events;

public sealed class InvasionWaveStarted(
    InvasionId aggregateId,
    int wave,
    int level,
    int ships,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public InvasionId InvasionId { get; } = aggregateId;

    public int Wave { get; } = wave;

    public int Level { get; } = level;

    public int Ships { get; } = ships;
}
