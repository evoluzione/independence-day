namespace Evoluzione.IndependenceDay.Contracts.Events;

/// <summary>
/// La nave e' stata abbattuta.
/// </summary>
/// <remarks>
/// I cannoni che le stavano sparando <b>continuano a sparare</b>: la Terra non li ferma, perche' non
/// e' lei a sapere se quel fuoco serviva ancora. Chi ha aperto il fuoco lo chiude.
/// </remarks>
public sealed class ShipDestroyed(
    EarthId aggregateId,
    ShipId shipId,
    CityId cityId,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public ShipId ShipId { get; } = shipId;
    public CityId CityId { get; } = cityId;
}
