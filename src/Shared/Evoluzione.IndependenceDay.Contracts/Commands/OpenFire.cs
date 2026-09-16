namespace Evoluzione.IndependenceDay.Contracts.Commands;

/// <summary>
/// Apri il fuoco su una nave: un cannone qualsiasi, purche' pronto.
/// </summary>
/// <remarks>
/// <b>Quale</b> cannone non lo sceglie chi ordina. Lo sceglie la Terra, che e' l'unica a sapere chi e'
/// pronto, chi sta gia' sparando a qualcun altro, chi e' inceppato e a chi restano munizioni. Chi
/// coordina dice soltanto che quella nave va presa di mira, e si sente rispondere con quale citta' —
/// oppure che non c'e' nessun cannone libero.
/// <para>
/// Il fuoco, una volta aperto, <b>non si ferma da solo</b>: il cannone continua a sparare a quella
/// nave finche' non arriva un <see cref="CeaseFire"/>. Anche quando la nave e' gia' caduta.
/// </para>
/// </remarks>
public sealed class OpenFire(EarthId aggregateId, ShipId shipId, Guid correlationId, Account who)
    : DefenseCommand(aggregateId, correlationId, who)
{
    public ShipId ShipId { get; } = shipId;
}
