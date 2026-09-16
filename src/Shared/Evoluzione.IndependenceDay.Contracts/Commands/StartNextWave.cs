namespace Evoluzione.IndependenceDay.Contracts.Commands;

/// <summary>
/// Manda avanti l'ondata successiva, con le difese come le ha lasciate quella prima.
/// </summary>
/// <remarks>
/// E' il comando che tiene in piedi il gioco: nessun ripristino fra un livello e l'altro, quindi
/// quello che una saga ha speso resta speso. Non fa niente se l'ondata in corso non e' ancora finita.
/// </remarks>
public sealed class StartNextWave(InvasionId aggregateId, Guid correlationId, Account who)
    : DefenseCommand(aggregateId, correlationId, who)
{
    public InvasionId InvasionId { get; } = aggregateId;
}
