namespace Evoluzione.IndependenceDay.Contracts.Events;

/// <summary>
/// Quel cannone ha finito le munizioni. Per sempre: non si ricarica.
/// </summary>
/// <remarks>
/// Il fuoco si ferma da se'. Quella citta' non spara piu' per il resto della campagna, e ogni colpo
/// che le era stato fatto sprecare prima si sente adesso.
/// </remarks>
public sealed class CannonEmpty(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
}
