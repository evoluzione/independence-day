namespace Evoluzione.IndependenceDay.Space.Messages.Commands;

public sealed class EndInvasion(InvasionId aggregateId, int wave, Guid correlationId, Account who)
    : SpaceCommand(aggregateId, correlationId, who)
{
    public InvasionId InvasionId { get; } = aggregateId;
    public int Wave { get; } = wave;
}
