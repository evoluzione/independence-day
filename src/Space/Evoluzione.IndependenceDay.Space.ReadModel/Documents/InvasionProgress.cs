using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Evoluzione.IndependenceDay.Space.ReadModel.Documents;

public class InvasionProgress : IProjectionDocument
{
    [BsonId] public Guid Id { get; set; }
    public int Wave { get; set; }
    public int Level { get; set; }
    public int Ships { get; set; }
    public bool Running { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long ProjectionVersion { get; set; }
}
