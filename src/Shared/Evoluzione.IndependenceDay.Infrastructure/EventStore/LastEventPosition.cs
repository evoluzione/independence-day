using MongoDB.Bson.Serialization.Attributes;

namespace Evoluzione.IndependenceDay.Infrastructure.EventStore;

public class LastEventPosition
{
    [BsonId] public string Id { get; set; } = string.Empty;
    public ulong CommitPosition { get; set; }
    public ulong PreparePosition { get; set; }
}
