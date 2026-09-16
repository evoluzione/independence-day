namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

/// <summary>Quel cannone ha finito i colpi. Per sempre: non si ricarica.</summary>
public sealed class EarthCannonEmpty(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
}
