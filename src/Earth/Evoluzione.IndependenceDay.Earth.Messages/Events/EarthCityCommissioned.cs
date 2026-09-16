namespace Evoluzione.IndependenceDay.Earth.Messages.Events;

/// <summary>Una citta' in piedi, con il suo cannone carico.</summary>
public sealed class EarthCityCommissioned(
    EarthId aggregateId,
    CityId cityId,
    string name,
    int integrity,
    int rounds,
    Guid correlationId) : DomainEvent(aggregateId, correlationId)
{
    public CityId CityId { get; } = cityId;
    public string Name { get; } = name;
    public int Integrity { get; } = integrity;
    public int Rounds { get; } = rounds;
}
