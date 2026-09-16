namespace Evoluzione.IndependenceDay.Contracts.Events;

/// <summary>Il tempo e' scaduto: la nave ha toccato terra e la citta' ne ha pagato il prezzo.</summary>
public sealed class ShipLanded(
    EarthId aggregateId,
    ShipId shipId,
    CityId cityId,
    int damage,
    int integrityLeft,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public ShipId ShipId { get; } = shipId;
    public CityId CityId { get; } = cityId;
    public int Damage { get; } = damage;
    public int IntegrityLeft { get; } = integrityLeft;
}
