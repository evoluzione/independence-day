using MongoDB.Driver;
using Muflone.Eventstore.gRPC;
using Muflone.Eventstore.gRPC.Persistence;

namespace Evoluzione.IndependenceDay.Infrastructure.EventStore;

public class EventStorePositionRepository(IMongoDatabase database) : IEventStorePositionRepository
{
    private const string CollectionName = "last_event_positions";

    public async Task<IEventStorePosition> GetLastPosition()
    {
        var collection = database.GetCollection<LastEventPosition>(CollectionName);
        var stored = await collection.Find(p => p.Id == Constants.LastEventPositionKey).FirstOrDefaultAsync();

        if (stored is not null)
            return new EventStorePosition(stored.CommitPosition, stored.PreparePosition);

        stored = new LastEventPosition { Id = Constants.LastEventPositionKey };
        await collection.InsertOneAsync(stored);

        return new EventStorePosition(stored.CommitPosition, stored.PreparePosition);
    }

    public Task Save(IEventStorePosition position)
    {
        var collection = database.GetCollection<LastEventPosition>(CollectionName);

        return collection.UpdateOneAsync(
            Builders<LastEventPosition>.Filter.Eq(p => p.Id, Constants.LastEventPositionKey),
            Builders<LastEventPosition>.Update
                .Set(p => p.CommitPosition, position.CommitPosition)
                .Set(p => p.PreparePosition, position.PreparePosition),
            new UpdateOptions { IsUpsert = true });
    }
}
