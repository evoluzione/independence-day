namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

/// <summary>Quel cannone ha aperto il fuoco, e non smettera' finche' non glielo si dice.</summary>
public sealed class EarthFireOpened(
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
