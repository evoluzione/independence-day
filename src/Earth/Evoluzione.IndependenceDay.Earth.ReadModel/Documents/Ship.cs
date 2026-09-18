using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Evoluzione.IndependenceDay.Earth.ReadModel.Documents;

public class Ship : IProjectionDocument
{
    [BsonId] public Guid Id { get; set; }
    public Guid CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public ShipClass Class { get; set; }

    public int Hits { get; set; }

    public int Wave { get; set; }

    public string Status { get; set; } = "incoming";

    public Guid CorrelationId { get; set; }

    public DateTime DetectedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long ProjectionVersion { get; set; }
}
