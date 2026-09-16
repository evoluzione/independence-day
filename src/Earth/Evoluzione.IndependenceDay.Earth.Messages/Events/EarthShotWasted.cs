namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

/// <summary>
/// Un colpo sparato a una nave che non c'e' piu'.
/// </summary>
/// <remarks>
/// Il cannone sta ancora sparando perche' nessuno gli ha detto di smettere. Ogni riga di queste nel
/// diario e' un cessate il fuoco che non e' arrivato.
/// </remarks>
public sealed class EarthShotWasted(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    int roundsLeft,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
    public int RoundsLeft { get; } = roundsLeft;
}
