namespace Evoluzione.IndependenceDay.Contracts.Events;

/// <summary>
/// La Terra ha preso in carico una nave: da adesso accetta ordini di fuoco su di lei.
/// </summary>
/// <remarks>
/// E' questo, e non l'avvistamento dello Spazio, il segnale da cui parte un'intercettazione.
/// L'avvistamento dice che la nave esiste; questo dice che c'e' qualcuno pronto a sparargli. Partire
/// dall'avvistamento significa correre contro la presa in carico e vedersi ignorare il primo ordine.
/// </remarks>
public sealed class ShipDetected(
    EarthId aggregateId,
    ShipId shipId,
    CityId cityId,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public ShipId ShipId { get; } = shipId;

    /// <summary>La citta' che quella nave sta puntando. Non e' detto sia quella che le spara.</summary>
    public CityId CityId { get; } = cityId;
}
