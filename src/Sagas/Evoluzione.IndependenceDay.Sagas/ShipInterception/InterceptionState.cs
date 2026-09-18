using Muflone.Saga;

namespace Evoluzione.IndependenceDay.Sagas.ShipInterception;

public enum ShipFate
{
    InFlight = 0,
    Destroyed = 1,
    Landed = 2
}

public sealed class InterceptionState : SagaStateBase
{
    public Guid ShipId { get; set; }

    public Guid CityId { get; set; }

    public HashSet<Guid> Firing { get; set; } = [];

    public HashSet<Guid> Jammed { get; set; } = [];

    public ShipFate Fate { get; set; }
}
