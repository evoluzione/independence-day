namespace Evoluzione.IndependenceDay.Contracts.Events;

public sealed class InvasionEnded(InvasionId aggregateId, int wave, int level, Guid correlationId)
    : IntegrationEvent(aggregateId, correlationId)
{
    public int Wave { get; } = wave;
    public int Level { get; } = level;
}
