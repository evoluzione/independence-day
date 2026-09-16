using Muflone.Core;

namespace Evoluzione.IndependenceDay.Space.Messages.Commands;

/// <summary>
/// Base dei comandi dello Spazio: commitId sempre nuovo, correlationId a parte.
/// </summary>
/// <remarks>Stessa ragione della Terra: il commitId e' l'identita' dell'append su EventStore.</remarks>
public abstract class SpaceCommand(IDomainId aggregateId, Guid correlationId, Account who)
    : Command(aggregateId, Guid.NewGuid(), who)
{
    public Guid CorrelationId { get; } = correlationId;
}
