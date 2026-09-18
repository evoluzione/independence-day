using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.ReadModel.Documents;
using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Evoluzione.IndependenceDay.Earth.ReadModel;

public sealed class BattleService : ProjectionPersister<City>, IBattleService
{
    private const int LogPageSize = 150;

    private static readonly TimeSpan ResolvedShipLinger = TimeSpan.FromSeconds(90);

    private readonly IMongoCollection<Ship> _ships;
    private readonly IMongoCollection<BattleLogEntry> _log;
    private readonly IMongoCollection<BattleState> _state;

    public BattleService([FromKeyedServices("earth-mongodb")] IMongoDatabase database) : base(database)
    {
        _ships = database.GetCollection<Ship>(nameof(Ship));
        _log = database.GetCollection<BattleLogEntry>(nameof(BattleLogEntry));
        _state = database.GetCollection<BattleState>(nameof(BattleState));
    }

    public Task<BattleState?> State(CancellationToken ct = default) =>
        _state.Find(x => x.Id == BattleState.Key).FirstOrDefaultAsync(ct)!;

    public Task WaveStarted(int wave, int ships, DateTime at, CancellationToken ct = default) =>
        _state.UpdateOneAsync(
            Builders<BattleState>.Filter.Eq(x => x.Id, BattleState.Key),
            Builders<BattleState>.Update
                .Set(x => x.Wave, wave)
                .Set(x => x.Ships, ships)
                .Set(x => x.Running, true)
                .Set(x => x.StartedAt, at)
                .Set(x => x.UpdatedAt, at),
            new UpdateOptions { IsUpsert = true }, ct);

    public Task WaveEnded(int wave, DateTime at, CancellationToken ct = default) =>
        _state.UpdateOneAsync(
            Builders<BattleState>.Filter.And(
                Builders<BattleState>.Filter.Eq(x => x.Id, BattleState.Key),
                Builders<BattleState>.Filter.Eq(x => x.Wave, wave)),
            Builders<BattleState>.Update.Set(x => x.Running, false).Set(x => x.UpdatedAt, at),
            cancellationToken: ct);

    public Task CommissionCity(Guid cityId, string name, int integrity, int rounds, DateTime at, long revision,
        CancellationToken ct = default) =>
        TryApply(cityId, revision,
            Builders<City>.Update
                .Set(x => x.Name, name)
                .SetOnInsert(x => x.Integrity, integrity)
                .SetOnInsert(x => x.Rounds, rounds)
                .SetOnInsert(x => x.Cannon, "ready")
                .SetOnInsert(x => x.Target, Guid.Empty),
            upsert: true, at, ct);

    public Task ResetCities(int integrity, int rounds, DateTime at, CancellationToken ct = default) =>
        Collection.UpdateManyAsync(FilterDefinition<City>.Empty,
            Builders<City>.Update
                .Set(x => x.Integrity, integrity)
                .Set(x => x.Rounds, rounds)
                .Set(x => x.Cannon, "ready")
                .Set(x => x.Target, Guid.Empty)
                .Set(x => x.UpdatedAt, at),
            cancellationToken: ct);

    public Task SetIntegrity(Guid cityId, int integrity, DateTime at, long revision,
        CancellationToken ct = default) =>
        TryApply(cityId, revision, Builders<City>.Update.Set(x => x.Integrity, integrity), upsert: false, at, ct);

    public Task SetCannon(Guid cityId, string status, Guid target, DateTime at, CancellationToken ct = default) =>
    Collection.UpdateOneAsync(
        Builders<City>.Filter.Eq(x => x.Id, cityId),
        Builders<City>.Update.Set(x => x.Cannon, status).Set(x => x.Target, target).Set(x => x.UpdatedAt, at),
        cancellationToken: ct);

    public Task SetRounds(Guid cityId, int rounds, DateTime at, CancellationToken ct = default) =>
        Collection.UpdateOneAsync(
            Builders<City>.Filter.Eq(x => x.Id, cityId),
            Builders<City>.Update.Set(x => x.Rounds, rounds).Set(x => x.UpdatedAt, at),
            cancellationToken: ct);

    public Task MarkShot(Guid cityId, int rounds, DateTime at, CancellationToken ct = default) =>
        Collection.UpdateOneAsync(
            Builders<City>.Filter.Eq(x => x.Id, cityId),
            Builders<City>.Update
                .Set(x => x.Rounds, rounds).Set(x => x.LastShotAt, at).Set(x => x.UpdatedAt, at),
            cancellationToken: ct);

    public Task SetCannonStatus(Guid cityId, string status, DateTime at, CancellationToken ct = default) =>
    Collection.UpdateOneAsync(
        Builders<City>.Filter.Eq(x => x.Id, cityId),
        Builders<City>.Update.Set(x => x.Cannon, status).Set(x => x.UpdatedAt, at),
        cancellationToken: ct);

    public async Task<IReadOnlyList<City>> FiringCannons(CancellationToken ct = default) =>
        await Collection.Find(c => c.Cannon == "firing").ToListAsync(ct);

    public async Task<IReadOnlyList<City>> EngagedCannons(CancellationToken ct = default) =>
    await Collection.Find(c => c.Cannon == "firing" && c.Target != Guid.Empty).ToListAsync(ct);

    public Task MarkResupply(Guid cityId, Guid shipId, DateTime at, CancellationToken ct = default) =>
        Collection.UpdateOneAsync(
            Builders<City>.Filter.Eq(x => x.Id, cityId),
            Builders<City>.Update.Set(x => x.ResupplyFor, shipId).Set(x => x.ResupplyAt, at)
                .Set(x => x.UpdatedAt, at),
            cancellationToken: ct);

    public async Task<IReadOnlyList<City>> ResupplyingCannons(CancellationToken ct = default) =>
        await Collection.Find(c => c.Cannon == "resupplying").ToListAsync(ct);

    public async Task DetectShip(Guid shipId, Guid cityId, ShipClass shipClass, int wave, Guid correlationId,
        DateTime at, long revision, CancellationToken ct = default)
    {
        var filter = Builders<Ship>.Filter.And(
            Builders<Ship>.Filter.Eq(x => x.Id, shipId),
            Builders<Ship>.Filter.Lt(x => x.ProjectionVersion, revision));

        await _ships.UpdateOneAsync(filter,
            Builders<Ship>.Update
                .Set(x => x.CityId, cityId)
                .Set(x => x.CityName, Cities.NameOf(cityId))
                .Set(x => x.Class, shipClass)
                .Set(x => x.Wave, wave)
                .Set(x => x.CorrelationId, correlationId)
                .Set(x => x.UpdatedAt, at)
                .Set(x => x.ProjectionVersion, revision)
                .SetOnInsert(x => x.Hits, 0)
                .SetOnInsert(x => x.Status, "incoming")
                .SetOnInsert(x => x.DetectedAt, at),
            new UpdateOptions { IsUpsert = true }, ct);
    }

    public Task SetHits(Guid shipId, int hits, DateTime at, CancellationToken ct = default) =>
    _ships.UpdateOneAsync(
        Builders<Ship>.Filter.Eq(x => x.Id, shipId),
        Builders<Ship>.Update.Set(x => x.Hits, hits).Set(x => x.UpdatedAt, at),
        cancellationToken: ct);

    public async Task CloseShip(Guid shipId, string status, DateTime at, CancellationToken ct = default)
    {
        var filter = Builders<Ship>.Filter.And(
            Builders<Ship>.Filter.Eq(x => x.Id, shipId),
            Builders<Ship>.Filter.Eq(x => x.Status, "incoming"));

        await _ships.UpdateOneAsync(filter,
            Builders<Ship>.Update.Set(x => x.Status, status).Set(x => x.UpdatedAt, at),
            cancellationToken: ct);
    }

    public Task<Ship?> Ship(Guid shipId, CancellationToken ct = default) =>
        _ships.Find(s => s.Id == shipId).FirstOrDefaultAsync(ct)!;

    public async Task<IReadOnlyList<Ship>> OpenShips(CancellationToken ct = default) =>
        await _ships.Find(s => s.Status == "incoming").ToListAsync(ct);

    public async Task Log(Guid shipId, string step, string detail, Guid cityId, string tone, DateTime at,
        CancellationToken ct = default, int amount = 0)
    {
        var wave = (await State(ct))?.Wave ?? 0;

        await LogInWave(shipId, step, detail, cityId, tone, wave, at, ct, amount);
    }

    public Task LogInWave(Guid shipId, string step, string detail, Guid cityId, string tone, int wave,
        DateTime at, CancellationToken ct = default, int amount = 0) =>
        _log.InsertOneAsync(new BattleLogEntry
        {
            Id = Guid.NewGuid(),
            At = at,
            Step = step,
            Detail = detail,
            CityName = Cities.NameOf(cityId),
            ShipId = shipId,
            Tone = tone,
            Wave = wave,
            Amount = amount
        }, cancellationToken: ct);

    public async Task<BattleSnapshot> GetSnapshot(CancellationToken ct = default)
    {
        var state = await State(ct);
        var wave = state?.Wave ?? 0;
        var now = DateTime.UtcNow;

        var cities = await Collection.Find(FilterDefinition<City>.Empty).ToListAsync(ct);

        var ships = await _ships.Find(s => s.Wave == wave).ToListAsync(ct);
        var log = await _log.Find(e => e.Wave == wave)
            .SortByDescending(x => x.At).Limit(LogPageSize).ToListAsync(ct);

        var cityViews = cities
            .OrderBy(city => city.Name)
            .Select(city => new CityView(city.Id, city.Name, city.Integrity, city.Integrity <= 0, city.Rounds,
                city.Cannon, city.Target))
            .ToList();

        var incoming = ships.Where(s => s.Status == "incoming").ToList();
        var landed = ships.Where(s => s.Status == "landed").ToList();
        var destroyed = ships.Where(s => s.Status == "destroyed").ToList();

        var steps = await CountBySteps(wave, ct);

        var since = now - ResolvedShipLinger;
        var approach = Invasion.ApproachSeconds * 1000;
        var shipViews = ships
            .Where(s => s.Status == "incoming" || s.UpdatedAt >= since)
            .OrderBy(s => s.DetectedAt)
            .Select(s => new ShipView(s.Id, s.CityId, s.CityName,
                Contracts.World.Ships.NameOf(s.Class), s.Hits, Contracts.World.Ships.HitsToDestroy(s.Class),
                s.Status, s.DetectedAt,
                Math.Max(0, approach - (int)(now - s.DetectedAt).TotalMilliseconds),
                cities.Count(c => c.Cannon == "firing" && c.Target == s.Id)))
            .ToList();

        var totalShips = state?.Ships ?? 0;
        var standing = cityViews.Count(c => !c.Fallen);
        var roundsLeft = cities.Sum(c => c.Integrity > 0 ? c.Rounds : 0);

        var started = state is not null;
        var gameOver = started && standing == 0;
        var over = gameOver || (started && !state!.Running && incoming.Count == 0);
        var campaignWon = over && standing > 0;

        var verdict = !over ? ""
            : gameOver ? "La Terra e' caduta."
            : landed.Count == 0 ? "Invasione respinta: nemmeno una nave ha toccato terra."
            : $"Invasione respinta: {standing} citta' su {cityViews.Count} ancora in piedi.";

        var status = !started ? "idle" : over ? "over" : "running";

        var endedAt = state?.Running == true ? now : state?.UpdatedAt ?? now;

        WaveSummary? summary = over
            ? new WaveSummary(
                state!.Wave, campaignWon, verdict,
                state.StartedAt, endedAt,
                (int)Math.Max(0, (endedAt - state.StartedAt).TotalSeconds),
                ships.Count, destroyed.Count, landed.Count,
                steps.GetValueOrDefault("shot") + steps.GetValueOrDefault("missed") +
                steps.GetValueOrDefault("wasted"),
                steps.GetValueOrDefault("wasted"),
                roundsLeft,
                standing, cityViews.Count, gameOver, campaignWon, steps, cityViews)
            : null;

        return new BattleSnapshot(
            new EarthState(standing, cityViews.Count, roundsLeft, cityViews),
            new AlienState(incoming.Count, destroyed.Count, landed.Count),
            new InvasionView(wave, status, totalShips,
                Math.Max(0, totalShips - destroyed.Count - landed.Count),
                over, gameOver, campaignWon, verdict, summary),
            steps,
            shipViews,
            log.Select(entry => new LogView(entry.Id, entry.At, entry.Step, entry.Detail, entry.CityName,
                entry.Tone)).ToList());
    }

    private async Task<Dictionary<string, int>> CountBySteps(int wave, CancellationToken ct)
    {
        var grouped = await _log.Aggregate()
            .Match(entry => entry.Wave == wave)
            .Group(entry => entry.Step, group => new { Step = group.Key, Count = group.Count() })
            .ToListAsync(ct);

        return grouped.ToDictionary(row => row.Step, row => row.Count);
    }
}
