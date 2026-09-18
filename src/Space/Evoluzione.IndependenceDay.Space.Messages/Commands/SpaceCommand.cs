using Muflone.Core;

namespace Evoluzione.IndependenceDay.Space.Messages.Commands;

public abstract class SpaceCommand(IDomainId aggregateId, Guid correlationId, Account who)
    : Command(aggregateId, Guid.NewGuid(), who)
{
    public Guid CorrelationId { get; } = correlationId;
}
