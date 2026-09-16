namespace Evoluzione.IndependenceDay.Contracts.Events;

/// <summary>Una nuova ondata e' partita, con quante navi.</summary>
public sealed class InvasionStarted(
    InvasionId aggregateId,
    int wave,
    int level,
    int ships,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    /// <summary>Progressivo di sempre: e' la chiave con cui read model e diario si filtrano.</summary>
    public int Wave { get; } = wave;

    /// <summary>Posizione nella campagna in corso, e quindi la difficolta'. E' il punteggio.</summary>
    public int Level { get; } = level;

    /// <summary>Quante navi porta l'ondata, di tutte le stazze.</summary>
    public int Ships { get; } = ships;
}
