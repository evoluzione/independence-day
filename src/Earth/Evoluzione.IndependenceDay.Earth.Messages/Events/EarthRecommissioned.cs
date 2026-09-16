namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

/// <summary>Campagna nuova: cannoni a pieno e citta' rimesse in piedi.</summary>
public sealed class EarthRecommissioned(
    EarthId aggregateId,
    int rounds,
    int wave,
    int integrity,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public int Rounds { get; } = rounds;
    public int Wave { get; } = wave;
    public int Integrity { get; } = integrity;
}
