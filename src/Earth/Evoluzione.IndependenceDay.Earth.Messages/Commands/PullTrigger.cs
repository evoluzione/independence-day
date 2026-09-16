namespace Evoluzione.IndependenceDay.Earth.Messages.Commands;

/// <summary>
/// Un colpo, se e' passato il tempo di ricarica.
/// </summary>
/// <remarks>
/// Lo manda la centrale di tiro della Terra, non chi coordina: la cadenza di un cannone la conosce
/// chi lo usa. Chi coordina apre e chiude il fuoco, e basta.
/// </remarks>
public sealed class PullTrigger(
    EarthId aggregateId,
    CityId cityId,
    Guid correlationId,
    Account who) : Command(aggregateId, Guid.NewGuid(), who)
{
    public CityId CityId { get; } = cityId;
    public Guid CorrelationId { get; } = correlationId;
}
