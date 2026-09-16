using Evoluzione.IndependenceDay.Contracts.World;

namespace Evoluzione.IndependenceDay.Space.Facade;

public interface ISpaceFacade
{
    Task<Guid> LaunchShip(Guid targetCityId, ShipClass shipClass, int wave, CancellationToken ct = default);
}
