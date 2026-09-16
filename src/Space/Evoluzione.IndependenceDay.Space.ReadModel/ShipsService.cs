using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using Evoluzione.IndependenceDay.Space.ReadModel.Documents;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Evoluzione.IndependenceDay.Space.ReadModel;

public sealed class ShipsService([FromKeyedServices("space-mongodb")] IMongoDatabase database)
    : ProjectionPersister<Ship>(database), IShipsService
{
    public Task Launch(Guid shipId, Guid targetCityId, Guid motherShipId, ShipClass shipClass, int wave,
        DateTime launchedAt, long revision, CancellationToken ct = default) =>
        TryApply(
            shipId,
            revision,
            Builders<Ship>.Update
                .Set(x => x.TargetCityId, targetCityId)
                .Set(x => x.MotherShipId, motherShipId)
                .Set(x => x.Class, shipClass)
                .Set(x => x.Wave, wave)
                .Set(x => x.Status, "approaching")
                .SetOnInsert(x => x.LaunchedAt, launchedAt),
            upsert: true,
            launchedAt,
            ct);

    public Task Close(Guid shipId, string status, string outcome, DateTime editDate, long revision,
        CancellationToken ct = default) =>
        TryApply(
            shipId,
            revision,
            Builders<Ship>.Update
                .Set(x => x.Status, status)
                .Set(x => x.Outcome, outcome),
            upsert: false,
            editDate,
            ct);

    public async Task<int> ShipsLaunched(int wave, CancellationToken ct = default) =>
        (int)await Collection.CountDocumentsAsync(f => f.Wave == wave, cancellationToken: ct);

    public async Task<int> ShipsInFlight(int wave, CancellationToken ct = default) =>
        (int)await Collection.CountDocumentsAsync(f => f.Wave == wave && f.Status == "approaching",
            cancellationToken: ct);

    public async Task<IReadOnlyList<Ship>> GetAll(CancellationToken ct = default) =>
        await Collection.Find(FilterDefinition<Ship>.Empty)
            .SortByDescending(x => x.LaunchedAt)
            .Limit(50)
            .ToListAsync(ct);
}
