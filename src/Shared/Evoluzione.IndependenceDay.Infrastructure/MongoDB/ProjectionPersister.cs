using MongoDB.Driver;

namespace Evoluzione.IndependenceDay.Infrastructure.MongoDB;

/// <summary>
/// Scritture di proiezione che rifiutano di essere sorpassate.
/// </summary>
/// <remarks>
/// Due eventi dello stesso aggregato possono arrivare al read model fuori ordine: chi atterra per
/// ultimo vince, e non e' detto sia quello accaduto per ultimo — una flotta respinta tornerebbe "in
/// avvicinamento". Ogni scrittura porta con se' la posizione dell'evento sul log e si applica solo se
/// il documento e' fermo a una posizione precedente.
/// </remarks>
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
        // Senza posizione sul log non c'e' niente da confrontare: si scrive e si lascia la versione
        // dov'era, cosi' la prossima scrittura versionata trova ancora il riferimento giusto.
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
