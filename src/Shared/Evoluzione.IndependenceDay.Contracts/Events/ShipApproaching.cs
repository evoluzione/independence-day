namespace Evoluzione.IndependenceDay.Contracts.Events;

/// <summary>
/// Il battito: questa nave e' ancora in volo, le manca tanto cosi', e le stanno sparando in tanti.
/// </summary>
/// <remarks>
/// La Terra lo emette a intervalli regolari per ogni nave ancora in volo, e non aspetta che nessuno
/// glielo chieda. E' l'unico orologio a disposizione di chi coordina, ed e' l'unico modo di
/// accorgersi del <b>silenzio</b>: un ordine perso non produce nessun evento, quindi non c'e' niente
/// da intercettare — c'e' solo una nave che al battito dopo risulta ancora viva con zero cannoni
/// addosso.
/// <para>
/// Non dice quanti colpi manchino ad abbatterla: quello e' un conto di dominio, e chi coordina non
/// deve farlo. Dice quanto tempo resta e quanti cannoni stanno lavorando, che e' quanto serve per
/// decidere se insistere o rinforzare.
/// </para>
/// </remarks>
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

    /// <summary>Quanto manca all'impatto, in millisecondi. Sotto zero e' gia' in ritardo.</summary>
    public int MsToImpact { get; } = msToImpact;

    /// <summary>Quanti cannoni le stanno sparando in questo momento.</summary>
    public int CannonsFiring { get; } = cannonsFiring;
}
