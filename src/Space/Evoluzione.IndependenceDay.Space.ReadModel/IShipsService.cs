using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Space.ReadModel.Documents;

namespace Evoluzione.IndependenceDay.Space.ReadModel;

public interface IShipsService
{
    Task Launch(Guid shipId, Guid targetCityId, Guid motherShipId, ShipClass shipClass, int wave, DateTime launchedAt,
        long revision, CancellationToken ct = default);

    Task Close(Guid shipId, string status, string outcome, DateTime editDate, long revision,
        CancellationToken ct = default);

    Task<IReadOnlyList<Ship>> GetAll(CancellationToken ct = default);

    Task<int> ShipsLaunched(int wave, CancellationToken ct = default);

    Task<int> ShipsInFlight(int wave, CancellationToken ct = default);
}
