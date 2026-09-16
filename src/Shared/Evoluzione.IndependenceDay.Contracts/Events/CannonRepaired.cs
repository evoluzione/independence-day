namespace Evoluzione.IndependenceDay.Contracts.Events;

/// <summary>
/// Il cannone e' tornato in sesto ed e' di nuovo disponibile.
/// </summary>
/// <remarks>
/// Disponibile, non in azione: riparare non riapre il fuoco. Chi lo ha fatto riparare deve anche
/// rimetterlo a sparare, altrimenti ha speso munizioni per un cannone che resta fermo.
/// </remarks>
public sealed class CannonRepaired(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    int roundsLeft,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
    public int RoundsLeft { get; } = roundsLeft;
}
