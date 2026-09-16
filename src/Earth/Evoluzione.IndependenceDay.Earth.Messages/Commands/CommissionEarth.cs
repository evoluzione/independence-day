namespace Evoluzione.IndependenceDay.Earth.Messages.Commands;

/// <summary>Mette in piedi la difesa: cinque citta', un cannone per citta'. Interno alla Terra.</summary>
public sealed class CommissionEarth(
    EarthId aggregateId,
    int rounds,
    int integrity,
    Guid correlationId,
    Account who) : Command(aggregateId, Guid.NewGuid(), who)
{
    /// <summary>Colpi in dotazione a ogni cannone per tutta la campagna.</summary>
    public int Rounds { get; } = rounds;

    public int Integrity { get; } = integrity;
    public Guid CorrelationId { get; } = correlationId;
}
