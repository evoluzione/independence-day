namespace Evoluzione.IndependenceDay.Contracts.Events;

/// <summary>
/// Questo cannone sta sparando a una nave che non c'e' piu'.
/// </summary>
/// <remarks>
/// La Terra lo ripete a intervalli regolari finche' dura, ed e' l'unico modo di accorgersi che un
/// cessate il fuoco si e' perso per strada: la nave e' risolta, quindi il battito
/// <see cref="ShipApproaching"/> non arriva piu', ma il cannone e' ancora li' a bruciare colpi.
/// <para>
/// Ogni riga di queste e' un colpo buttato e un cannone che manca alla nave successiva. Non e'
/// un'informazione di servizio: e' la sveglia.
/// </para>
/// </remarks>
public sealed class CannonStillFiring(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
}
