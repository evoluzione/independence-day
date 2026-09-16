using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using Evoluzione.IndependenceDay.Space.ReadModel.Documents;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Evoluzione.IndependenceDay.Space.ReadModel;

public sealed class InvasionProgressService([FromKeyedServices("space-mongodb")] IMongoDatabase database)
    : ProjectionPersister<InvasionProgress>(database), IInvasionProgressService
{
    public Task WaveStarted(Guid invasionId, int wave, int level, int ships, DateTime at,
        long revision, CancellationToken ct = default) =>
        TryApply(invasionId, revision,
            Builders<InvasionProgress>.Update
                .Set(x => x.Wave, wave)
                .Set(x => x.Level, level)
                .Set(x => x.Ships, ships)
                .Set(x => x.Running, true)
                .Set(x => x.StartedAt, at),
            upsert: true, at, ct);

    public Task WaveEnded(Guid invasionId, DateTime at, long revision, CancellationToken ct = default) =>
        TryApply(invasionId, revision, Builders<InvasionProgress>.Update.Set(x => x.Running, false),
            upsert: false, at, ct);

    public Task<InvasionProgress?> Current(Guid invasionId, CancellationToken ct = default) =>
        Collection.Find(x => x.Id == invasionId).FirstOrDefaultAsync(ct)!;
}
