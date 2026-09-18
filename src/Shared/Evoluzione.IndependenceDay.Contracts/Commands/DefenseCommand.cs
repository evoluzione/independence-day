using Muflone.Core;

namespace Evoluzione.IndependenceDay.Contracts.Commands;

public abstract class DefenseCommand(IDomainId aggregateId, Guid correlationId, Account who)
    : Command(aggregateId, Guid.NewGuid(), who)
{
    public Guid CorrelationId { get; } = correlationId;
}
