namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

/// <summary>
/// L'integrita' e' a zero: la citta' e' perduta.
/// </summary>
/// <remarks>
/// Con lei se ne va il suo cannone, e i colpi che gli restavano. Perdere una citta' non e' solo
/// perdere un punto: e' perdere un quinto della potenza di fuoco per tutto il resto della campagna.
/// </remarks>
public sealed class EarthCityFallen(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
}
