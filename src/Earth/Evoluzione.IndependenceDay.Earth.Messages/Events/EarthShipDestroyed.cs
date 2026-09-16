namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

/// <summary>La nave e' stata abbattuta. I cannoni puntati su di lei continuano a sparare.</summary>
public sealed class EarthShipDestroyed(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    /// <summary>La citta' che la nave stava puntando, non quella che l'ha abbattuta.</summary>
    public CityId CityId { get; } = cityId;

    public ShipId ShipId { get; } = shipId;
}
