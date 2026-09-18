using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Muflone.Saga.Persistence;

namespace Evoluzione.IndependenceDay.Sagas.Infrastructure.MongoDB;

public sealed class MongoSagaRepository(
    [FromKeyedServices("sagas-mongodb")] IMongoDatabase database) : ISagaRepository
{
    private readonly IMongoCollection<BsonDocument> _collection =
        database.GetCollection<BsonDocument>(MongoSagaCollection.Name);

    public Task SaveAsync<TSagaState>(Guid correlationId, TSagaState sagaState) where TSagaState : class, new()
    {
        var key = correlationId.ToString("N");
        var document = new BsonDocument
        {
            ["_id"] = key,
            ["stateType"] = StateType<TSagaState>(),
            ["updatedAtUtc"] = DateTime.UtcNow,
            ["state"] = sagaState.ToBsonDocument()
        };

        return _collection.ReplaceOneAsync(
            Builders<BsonDocument>.Filter.Eq(MongoSagaCollection.CorrelationIdField, key),
            document,
            new ReplaceOptions { IsUpsert = true });
    }

    public async Task<TSagaState> GetByIdAsync<TSagaState>(Guid id) where TSagaState : class, new()
    {
        var key = id.ToString("N");
        var filter = Builders<BsonDocument>.Filter.And(
            Builders<BsonDocument>.Filter.Eq(MongoSagaCollection.CorrelationIdField, key),
            Builders<BsonDocument>.Filter.Eq(MongoSagaCollection.StateTypeField, StateType<TSagaState>()));

        var document = await _collection.Find(filter).FirstOrDefaultAsync();

        if (document is null || !document.TryGetValue("state", out var state) || !state.IsBsonDocument)
            return null!;

        return BsonSerializer.Deserialize<TSagaState>(state.AsBsonDocument);
    }

    public Task CompleteAsync(Guid correlationId) =>
        _collection.DeleteOneAsync(
            Builders<BsonDocument>.Filter.Eq(MongoSagaCollection.CorrelationIdField, correlationId.ToString("N")));

    internal static string StateType<TSagaState>() where TSagaState : class, new() =>
        typeof(TSagaState).AssemblyQualifiedName ?? typeof(TSagaState).FullName ?? typeof(TSagaState).Name;
}
