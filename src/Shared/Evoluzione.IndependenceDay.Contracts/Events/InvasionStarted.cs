namespace Evoluzione.IndependenceDay.Contracts.Events;

public sealed class InvasionStarted(
    InvasionId aggregateId,
    int wave,
    int level,
    int ships,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public int Wave { get; } = wave;

    public int Level { get; } = level;

    public int Ships { get; } = ships;
}
