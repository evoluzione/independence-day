namespace Evoluzione.IndependenceDay.Earth.Messages.Commands;

/// <summary>Riporta tutto a nuovo per una campagna. Non succede fra un livello e l'altro.</summary>
public sealed class RecommissionEarth(
    EarthId aggregateId,
    int rounds,
    int wave,
    int integrity,
    Guid correlationId,
    Account who) : Command(aggregateId, Guid.NewGuid(), who)
{
    public int Rounds { get; } = rounds;
    public int Wave { get; } = wave;
    public int Integrity { get; } = integrity;
    public Guid CorrelationId { get; } = correlationId;
}
