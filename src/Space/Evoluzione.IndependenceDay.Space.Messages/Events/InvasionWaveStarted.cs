namespace Evoluzione.IndependenceDay.Space.Messages.Events;

public sealed class InvasionWaveStarted(
    InvasionId aggregateId,
    int wave,
    int level,
    int ships,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public InvasionId InvasionId { get; } = aggregateId;

    /// <summary>Progressivo di sempre, anche fra una campagna e l'altra: non si ripete mai.</summary>
    public int Wave { get; } = wave;

    /// <summary>Posizione nella campagna, e quindi la difficolta'.</summary>
    public int Level { get; } = level;

    /// <summary>Quante navi porta l'ondata, di tutte le stazze.</summary>
    public int Ships { get; } = ships;
}
