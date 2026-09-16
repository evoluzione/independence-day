using Muflone.Core;

namespace Evoluzione.IndependenceDay.Contracts.Commands;

/// <summary>
/// Base dei comandi che la saga manda alla Terra.
/// </summary>
/// <remarks>
/// Il <c>commitId</c> e' sempre nuovo e il correlationId viaggia a parte, come proprieta'.
/// Il motivo e' che il commitId finisce per essere l'identita' dell'append su EventStore: quattro
/// comandi della stessa saga vanno tutti sullo stesso aggregato citta', e se portassero lo stesso
/// commitId l'event store scarterebbe i successivi come riconsegne — in silenzio, senza errore,
/// lasciando la difesa ferma al primo passo.
/// </remarks>
public abstract class DefenseCommand(IDomainId aggregateId, Guid correlationId, Account who)
    : Command(aggregateId, Guid.NewGuid(), who)
{
    public Guid CorrelationId { get; } = correlationId;
}
