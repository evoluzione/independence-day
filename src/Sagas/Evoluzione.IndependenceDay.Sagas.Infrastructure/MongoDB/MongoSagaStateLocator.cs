using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Muflone.Saga;
using Muflone.Saga.Persistence;

namespace Evoluzione.IndependenceDay.Sagas.Infrastructure.MongoDB;

/// <summary>
/// Trova una saga aperta per chiave di business, quando un evento non puo' indirizzarla per
/// correlationId.
/// </summary>
/// <remarks>
/// Filtra sempre per <c>stateType</c>: la collection e' eterogenea e due stati diversi possono avere
/// una proprieta' con lo stesso nome. E filtra per stato aperto, perche' una saga fallita non va
/// riaperta da un evento in ritardo.
/// </remarks>
public sealed class MongoSagaStateLocator(
    [FromKeyedServices("sagas-mongodb")] IMongoDatabase database) : ISagaStateLocator
{
    private static readonly int[] OpenStatuses = [(int)SagaStatus.Started, (int)SagaStatus.InProgress];

    private readonly IMongoCollection<BsonDocument> _collection =
        database.GetCollection<BsonDocument>(MongoSagaCollection.Name);

    public async Task<IReadOnlyList<TSagaState>> FindOpenStatesBy<TSagaState>(
        string stateField,
        Guid value,
        CancellationToken ct = default) where TSagaState : SagaStateBase, new()
    {
        if (string.IsNullOrWhiteSpace(stateField) || value == Guid.Empty)
            return [];

        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq(MongoSagaCollection.StateTypeField,
                MongoSagaRepository.StateType<TSagaState>()),
            Builders<BsonDocument>.Filter.Eq(MongoSagaCollection.StateField(stateField), value),
            Builders<BsonDocument>.Filter.In(MongoSagaCollection.StatusField, OpenStatuses));

        var documents = await _collection.Find(filter).ToListAsync(ct);

        return documents
            .Where(document => document.TryGetValue("state", out var state) && state.IsBsonDocument)
            .Select(document => BsonSerializer.Deserialize<TSagaState>(document["state"].AsBsonDocument))
            .ToList();
    }
}
