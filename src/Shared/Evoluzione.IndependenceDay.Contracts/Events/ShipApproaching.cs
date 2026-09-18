namespace Evoluzione.IndependenceDay.Contracts.Events;

public sealed class ShipApproaching(
    EarthId aggregateId,
    ShipId shipId,
    CityId cityId,
    int msToImpact,
    int cannonsFiring,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public ShipId ShipId { get; } = shipId;
    public CityId CityId { get; } = cityId;

    public int MsToImpact { get; } = msToImpact;

    public int CannonsFiring { get; } = cannonsFiring;
}
