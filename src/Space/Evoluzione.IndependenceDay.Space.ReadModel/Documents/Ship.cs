using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Evoluzione.IndependenceDay.Space.ReadModel.Documents;

public class Ship : IProjectionDocument
{
    [BsonId] public Guid Id { get; set; }
    public Guid TargetCityId { get; set; }
    public Guid MotherShipId { get; set; }
    public ShipClass Class { get; set; }

    /// <summary>L'ondata a cui questa nave appartiene.</summary>
    public int Wave { get; set; }
    public string Status { get; set; } = string.Empty;

    /// <summary>Perche' la nave e' finita cosi': "nuked", "repelled", "landed".</summary>
    public string Outcome { get; set; } = string.Empty;

    public DateTime LaunchedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long ProjectionVersion { get; set; }
}
