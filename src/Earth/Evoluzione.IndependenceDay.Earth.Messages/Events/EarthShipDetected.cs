using Evoluzione.IndependenceDay.Contracts.World;

namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

/// <summary>Una nave e' stata presa in carico: da adesso la Terra accetta ordini di fuoco su di lei.</summary>
public sealed class EarthShipDetected(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    ShipClass shipClass,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
    public ShipClass ShipClass { get; } = shipClass;
}
