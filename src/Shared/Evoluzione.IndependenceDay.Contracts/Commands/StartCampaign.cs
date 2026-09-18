namespace Evoluzione.IndependenceDay.Contracts.Commands;

public sealed class StartCampaign(InvasionId aggregateId, Guid correlationId, Account who)
    : DefenseCommand(aggregateId, correlationId, who)
{
    public InvasionId InvasionId { get; } = aggregateId;
}
