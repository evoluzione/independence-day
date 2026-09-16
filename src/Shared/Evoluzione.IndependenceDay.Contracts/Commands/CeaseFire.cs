namespace Evoluzione.IndependenceDay.Contracts.Commands;

/// <summary>
/// Cessa il fuoco: quel cannone smette di sparare e torna disponibile.
/// </summary>
/// <remarks>
/// E' l'azione compensativa del processo, e la sola che rimetta indietro qualcosa. Un cannone che
/// nessuno ferma continua a sparare a una nave che non c'e' piu': brucia le munizioni della sua citta'
/// e, soprattutto, <b>non e' li'</b> quando arriva la nave successiva. Dimenticarla non produce
/// nessun errore — produce una citta' indifesa tre ondate piu' tardi.
/// </remarks>
public sealed class CeaseFire(EarthId aggregateId, CityId cityId, ShipId shipId, Guid correlationId, Account who)
    : DefenseCommand(aggregateId, correlationId, who)
{
    public CityId CityId { get; } = cityId;

    /// <summary>
    /// La nave a cui quel cannone stava sparando.
    /// </summary>
    /// <remarks>
    /// Serve perche' un cessate il fuoco in ritardo non spenga il cannone che nel frattempo e' stato
    /// riassegnato a un'altra nave.
    /// </remarks>
    public ShipId ShipId { get; } = shipId;
}
