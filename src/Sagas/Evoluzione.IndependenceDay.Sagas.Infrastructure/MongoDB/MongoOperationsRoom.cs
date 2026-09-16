using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Evoluzione.IndependenceDay.Sagas.Infrastructure.MongoDB;

/// <summary>Quante intercettazioni sono aperte in questo momento.</summary>
public interface IOperationsRoom
{
    Task<long> OpenLines(CancellationToken ct = default);
}

/// <summary>
/// Le linee occupate, contate dai documenti delle saghe.
/// </summary>
/// <remarks>
/// Un processo che si chiude cancella il suo documento, quindi contare i documenti e' contare i
/// processi aperti. Un processo che non si chiude mai lascia il suo li' per sempre, e da fuori non si
/// vede nessuna differenza fra "sta ancora lavorando" e "non ha mai finito" — se non che il numero
/// cresce e non torna mai indietro.
/// </remarks>
public sealed class MongoOperationsRoom(
    [FromKeyedServices("sagas-mongodb")] IMongoDatabase database) : IOperationsRoom
{
    private readonly IMongoCollection<BsonDocument> _collection =
        database.GetCollection<BsonDocument>(MongoSagaCollection.Name);

    public Task<long> OpenLines(CancellationToken ct = default) =>
        _collection.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, cancellationToken: ct);
}
