namespace Evoluzione.IndependenceDay.Contracts.Events;

/// <summary>
/// Il cannone si e' inceppato: il colpo non e' partito e il fuoco si e' fermato.
/// </summary>
/// <remarks>
/// Il cannone resta fuori uso finche' qualcuno non manda una riparazione, e non spara a nessun altro
/// nel frattempo. La nave che stava affrontando e' ancora in volo, e adesso non le spara piu' niente.
/// </remarks>
public sealed class CannonJammed(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;

    /// <summary>La nave a cui stava sparando quando si e' inceppato.</summary>
    public ShipId ShipId { get; } = shipId;
}
