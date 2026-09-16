namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

/// <summary>
/// Un colpo che manca il bersaglio: la munizione se n'e' andata, la nave regge.
/// </summary>
/// <remarks>
/// Non e' un guasto e non c'e' niente da fare: il fuoco e' aperto, quindi il cannone ricarica e
/// riprova da solo. Costa una munizione e, soprattutto, tiene occupato quel cannone piu' a lungo.
/// </remarks>
public sealed class EarthShotMissed(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    int roundsLeft,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;
    public int RoundsLeft { get; } = roundsLeft;
}
