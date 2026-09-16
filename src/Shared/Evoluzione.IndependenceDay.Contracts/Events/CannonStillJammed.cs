namespace Evoluzione.IndependenceDay.Contracts.Events;

/// <summary>
/// Questo cannone e' ancora inceppato.
/// </summary>
/// <remarks>
/// La Terra lo ripete a ogni battito finche' dura, ed e' il gemello di
/// <see cref="CannonStillFiring"/>: li' un cannone spara e non dovrebbe, qui un cannone non spara e
/// dovrebbe. Arriva per due ragioni, e non dice quale delle due:
/// <list type="bullet">
/// <item>l'ordine di riparare si e' perso per strada e alla Terra non e' mai arrivato;</item>
/// <item>l'ordine e' arrivato ma la riparazione <b>non ha preso</b> — succede, e i colpi spesi non
/// tornano indietro.</item>
/// </list>
/// <para>
/// In tutti e due i casi la risposta e' la stessa: insistere. Un cannone inceppato e dimenticato non
/// si ripara da solo, ed e' un quinto della potenza di fuoco che non c'e' piu' per il resto della
/// campagna.
/// </para>
/// </remarks>
public sealed class CannonStillJammed(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    Guid correlationId) : IntegrationEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
}
