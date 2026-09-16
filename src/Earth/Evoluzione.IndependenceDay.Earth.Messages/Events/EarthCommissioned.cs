namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

/// <summary>La difesa della Terra e' in piedi: quanti colpi ha ogni cannone.</summary>
public sealed class EarthCommissioned(EarthId aggregateId, int rounds, Guid correlationId)
    : DomainEvent(aggregateId, correlationId)
{
    public int Rounds { get; } = rounds;
}
