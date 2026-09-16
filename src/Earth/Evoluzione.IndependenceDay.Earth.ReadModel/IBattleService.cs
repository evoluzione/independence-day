using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.ReadModel.Documents;

namespace Evoluzione.IndependenceDay.Earth.ReadModel;

public interface IBattleService
{
    Task CommissionCity(Guid cityId, string name, int integrity, int rounds, DateTime at, long revision,
        CancellationToken ct = default);

    Task ResetCities(int integrity, int rounds, DateTime at, CancellationToken ct = default);

    Task SetIntegrity(Guid cityId, int integrity, DateTime at, long revision, CancellationToken ct = default);

    /// <summary>Lo stato del cannone, e su chi e' puntato.</summary>
    Task SetCannon(Guid cityId, string status, Guid target, DateTime at, CancellationToken ct = default);

    Task SetRounds(Guid cityId, int rounds, DateTime at, CancellationToken ct = default);

    /// <summary>Segna il colpo appena partito: e' da qui che si conta la ricarica.</summary>
    Task MarkShot(Guid cityId, int rounds, DateTime at, CancellationToken ct = default);

    /// <summary>I cannoni che stanno sparando: e' cosi' che la centrale di tiro sa a chi premere il grilletto.</summary>
    Task SetCannonStatus(Guid cityId, string status, DateTime at, CancellationToken ct = default);

    Task SetJammedOn(Guid cityId, Guid shipId, DateTime at, CancellationToken ct = default);

    Task<IReadOnlyList<City>> FiringCannons(CancellationToken ct = default);

    Task<IReadOnlyList<City>> EngagedCannons(CancellationToken ct = default);

    Task<IReadOnlyList<City>> JammedCannons(CancellationToken ct = default);

    Task DetectShip(Guid shipId, Guid cityId, ShipClass shipClass, int wave, Guid correlationId, DateTime at,
        long revision, CancellationToken ct = default);

    Task SetHits(Guid shipId, int hits, DateTime at, CancellationToken ct = default);

    Task CloseShip(Guid shipId, string status, DateTime at, CancellationToken ct = default);

    /// <summary>Una nave qualunque, anche gia' risolta: serve a ritrovarne la correlazione.</summary>
    Task<Ship?> Ship(Guid shipId, CancellationToken ct = default);

    /// <summary>Le navi ancora in volo: e' cosi' che si sa a chi mandare il battito e a chi e' scaduto il tempo.</summary>
    Task<IReadOnlyList<Ship>> OpenShips(CancellationToken ct = default);

    Task Log(Guid shipId, string step, string detail, Guid cityId, string tone, DateTime at,
        CancellationToken ct = default, int amount = 0);

    /// <summary>Scrive nel diario di un'ondata precisa, quando chi scrive la conosce gia'.</summary>
    Task LogInWave(Guid shipId, string step, string detail, Guid cityId, string tone, int wave, DateTime at,
        CancellationToken ct = default, int amount = 0);

    Task<BattleState?> State(CancellationToken ct = default);

    Task WaveStarted(int wave, int level, int ships, DateTime at, CancellationToken ct = default);

    Task WaveEnded(int wave, DateTime at, CancellationToken ct = default);

    Task<BattleSnapshot> GetSnapshot(CancellationToken ct = default);
}
