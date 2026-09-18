using MongoDB.Driver;

namespace Evoluzione.IndependenceDay.Infrastructure.MongoDB;

public abstract class ProjectionPersister<TDocument>(IMongoDatabase database)
    where TDocument : IProjectionDocument
{
    protected readonly IMongoCollection<TDocument> Collection =
        database.GetCollection<TDocument>(typeof(TDocument).Name);

    protected async Task<bool> TryApply(
        Guid id,
        long revision,
        UpdateDefinition<TDocument> update,
        bool upsert,
        DateTime editDate,
        CancellationToken ct = default)
    {

        var unversioned = revision < 0;

        var filter = unversioned
            ? Builders<TDocument>.Filter.Eq(x => x.Id, id)
            : Builders<TDocument>.Filter.And(
                Builders<TDocument>.Filter.Eq(x => x.Id, id),
                Builders<TDocument>.Filter.Lt(x => x.ProjectionVersion, revision));

        var versioned = unversioned
            ? Builders<TDocument>.Update.Combine(update,
                Builders<TDocument>.Update.Set(x => x.UpdatedAt, editDate))
            : Builders<TDocument>.Update.Combine(update,
                Builders<TDocument>.Update.Set(x => x.UpdatedAt, editDate),
                Builders<TDocument>.Update.Set(x => x.ProjectionVersion, revision));

        var result = await Collection.UpdateOneAsync(filter, versioned, new UpdateOptions { IsUpsert = upsert }, ct);

        return result.ModifiedCount > 0 || result.UpsertedId is not null;
    }
}
