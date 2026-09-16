namespace Evoluzione.IndependenceDay.Contracts.Commands.Sagas;

/// <summary>Il comando che accende il processo di intercettazione. L'aggregato e' la nave da fermare.</summary>
public sealed class StartShipInterception(
    ShipId aggregateId,
    CityId targetCity,
    Guid correlationId,
    Account who) : DefenseCommand(aggregateId, correlationId, who)
{
    public ShipId ShipId { get; } = aggregateId;
    public CityId TargetCity { get; } = targetCity;
}
