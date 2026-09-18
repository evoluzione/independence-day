using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.ReadModel.Documents;

namespace Evoluzione.IndependenceDay.Earth.ReadModel;

public interface IBattleService
{
    Task CommissionCity(Guid cityId, string name, int integrity, int rounds, DateTime at, long revision,
        CancellationToken ct = default);

    Task ResetCities(int integrity, int rounds, DateTime at, CancellationToken ct = default);

    Task SetIntegrity(Guid cityId, int integrity, DateTime at, long revision, CancellationToken ct = default);

    Task SetCannon(Guid cityId, string status, Guid target, DateTime at, CancellationToken ct = default);

    Task SetRounds(Guid cityId, int rounds, DateTime at, CancellationToken ct = default);

    Task MarkShot(Guid cityId, int rounds, DateTime at, CancellationToken ct = default);

    Task SetCannonStatus(Guid cityId, string status, DateTime at, CancellationToken ct = default);

    Task<IReadOnlyList<City>> FiringCannons(CancellationToken ct = default);

    Task<IReadOnlyList<City>> EngagedCannons(CancellationToken ct = default);

    Task MarkResupply(Guid cityId, Guid shipId, DateTime at, CancellationToken ct = default);

    Task<IReadOnlyList<City>> ResupplyingCannons(CancellationToken ct = default);

    Task DetectShip(Guid shipId, Guid cityId, ShipClass shipClass, int wave, Guid correlationId, DateTime at,
        long revision, CancellationToken ct = default);

    Task SetHits(Guid shipId, int hits, DateTime at, CancellationToken ct = default);

    Task CloseShip(Guid shipId, string status, DateTime at, CancellationToken ct = default);

    Task<Ship?> Ship(Guid shipId, CancellationToken ct = default);

    Task<IReadOnlyList<Ship>> OpenShips(CancellationToken ct = default);

    Task Log(Guid shipId, string step, string detail, Guid cityId, string tone, DateTime at,
        CancellationToken ct = default, int amount = 0);

    Task LogInWave(Guid shipId, string step, string detail, Guid cityId, string tone, int wave, DateTime at,
    CancellationToken ct = default, int amount = 0);

    Task<BattleState?> State(CancellationToken ct = default);

    Task WaveStarted(int wave, int ships, DateTime at, CancellationToken ct = default);

    Task WaveEnded(int wave, DateTime at, CancellationToken ct = default);

    Task<BattleSnapshot> GetSnapshot(CancellationToken ct = default);
}
