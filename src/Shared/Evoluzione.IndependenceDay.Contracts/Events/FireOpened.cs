namespace Evoluzione.IndependenceDay.Contracts.Events;

/// <summary>Un cannone ha aperto il fuoco su quella nave, e continuera' finche' non gli si dice basta.</summary>
public sealed class FireOpened(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    int roundsLeft,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    /// <summary>Il cannone che ha aperto il fuoco: l'ha scelto la Terra, non chi ha ordinato.</summary>
    public CityId CityId { get; } = cityId;

    public ShipId ShipId { get; } = shipId;

    /// <summary>Colpi che restano a quella citta' per il resto della campagna.</summary>
    public int RoundsLeft { get; } = roundsLeft;
}
