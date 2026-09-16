namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

/// <summary>Un colpo a segno. La nave regge o cade: lo dice l'evento dopo.</summary>
public sealed class EarthShotFired(
    EarthId aggregateId,
    CityId cityId,
    ShipId shipId,
    int hits,
    int roundsLeft,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public ShipId ShipId { get; } = shipId;

    /// <summary>Colpi incassati da quella nave finora.</summary>
    public int Hits { get; } = hits;

    public int RoundsLeft { get; } = roundsLeft;
}
