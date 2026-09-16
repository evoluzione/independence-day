namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

/// <summary>
/// Un ordine si e' perso per strada e non e' stato eseguito.
/// </summary>
/// <remarks>
/// Questo evento resta <b>dentro la Terra</b>: non esiste nessuna versione di integrazione, quindi
/// chi ha impartito l'ordine non riceve niente. E' il punto: un ordine perso non e' un errore da
/// intercettare, e' silenzio. Lo si vede solo nel diario a schermo, che serve a capire a posteriori
/// perche' una citta' e' rimasta scoperta.
/// <para>
/// Segnarlo comunque serve a due cose: far avanzare il contatore dei tentativi — cosi' due ordini di
/// fila non si perdono mai e riprovare basta sempre — e lasciare la traccia.
/// </para>
/// </remarks>
public sealed class EarthOrderLost(
    EarthId aggregateId,
    string order,
    CityId cityId,
    ShipId shipId,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    /// <summary>Quale ordine si e' perso: apertura del fuoco, cessate il fuoco, riparazione.</summary>
    public string Order { get; } = order;

    /// <summary>La citta' a cui era diretto, se ne aveva una gia' scelta.</summary>
    public CityId CityId { get; } = cityId;

    public ShipId ShipId { get; } = shipId;
}
