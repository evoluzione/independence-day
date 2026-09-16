namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

/// <summary>Il cannone e' tornato disponibile. Disponibile, non in azione.</summary>
public sealed class EarthCannonRepaired(
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
