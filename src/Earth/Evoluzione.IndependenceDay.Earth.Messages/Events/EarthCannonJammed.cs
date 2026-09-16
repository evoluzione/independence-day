namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

/// <summary>Il cannone si e' inceppato: il colpo non e' partito e il fuoco si e' fermato.</summary>
public sealed class EarthCannonJammed(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
}
