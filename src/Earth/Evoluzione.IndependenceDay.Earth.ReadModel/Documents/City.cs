using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Evoluzione.IndependenceDay.Earth.ReadModel.Documents;

public class City : IProjectionDocument
{
    [BsonId] public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int Integrity { get; set; }

    public int Rounds { get; set; }

    public string Cannon { get; set; } = "ready";

    public Guid Target { get; set; }

    public DateTime LastShotAt { get; set; }

    public DateTime ResupplyAt { get; set; }

    public Guid ResupplyFor { get; set; }

    public DateTime UpdatedAt { get; set; }
    public long ProjectionVersion { get; set; }
}
