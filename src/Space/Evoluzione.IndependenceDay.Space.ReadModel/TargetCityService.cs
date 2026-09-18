using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using Evoluzione.IndependenceDay.Space.ReadModel.Documents;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Evoluzione.IndependenceDay.Space.ReadModel;

public sealed class TargetCityService([FromKeyedServices("space-mongodb")] IMongoDatabase database)
    : ProjectionPersister<TargetCity>(database), ITargetCityService
{
    public async Task<IReadOnlyList<Guid>> Standing(CancellationToken ct = default)
    {
        var fallen = await Collection.Find(x => !x.Standing).Project(x => x.Id).ToListAsync(ct);

        return Cities.All.Select(c => c.Id).Where(id => !fallen.Contains(id)).ToList();
    }

    public Task MarkFallen(Guid cityId, DateTime at, CancellationToken ct = default) =>
    Collection.UpdateOneAsync(
        Builders<TargetCity>.Filter.And(
            Builders<TargetCity>.Filter.Eq(x => x.Id, cityId),
            Builders<TargetCity>.Filter.Lt(x => x.UpdatedAt, at)),
        Builders<TargetCity>.Update.Set(x => x.Standing, false).Set(x => x.UpdatedAt, at),
        cancellationToken: ct);

    public Task ResetAll(DateTime at, CancellationToken ct = default) =>
    Collection.BulkWriteAsync(
        Cities.All.Select(city => new ReplaceOneModel<TargetCity>(
            Builders<TargetCity>.Filter.Eq(x => x.Id, city.Id),
            new TargetCity { Id = city.Id, Standing = true, UpdatedAt = at })
        { IsUpsert = true }),
        cancellationToken: ct);
}
