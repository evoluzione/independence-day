using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Evoluzione.IndependenceDay.Space.ReadModel.Documents;

public class TargetCity : IProjectionDocument
{
    [BsonId] public Guid Id { get; set; }
    public bool Standing { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long ProjectionVersion { get; set; }
}
