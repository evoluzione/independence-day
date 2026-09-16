using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Evoluzione.IndependenceDay.Earth.ReadModel.Documents;

/// <summary>
/// Una nave aliena vista dalla Terra.
/// </summary>
/// <remarks>
/// La Terra non legge il read model dello Spazio: si costruisce il proprio quadro dagli eventi che
/// passano sul bus. E' la stessa nave, ma questa e' la copia che la Terra possiede.
/// </remarks>
public class Ship : IProjectionDocument
{
    [BsonId] public Guid Id { get; set; }
    public Guid CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public ShipClass Class { get; set; }

    /// <summary>Colpi gia' incassati.</summary>
    public int Hits { get; set; }

    /// <summary>L'ondata a cui appartiene.</summary>
    public int Wave { get; set; }

    /// <summary>incoming | destroyed | landed</summary>
    public string Status { get; set; } = "incoming";

    /// <summary>
    /// La correlazione con cui la nave e' stata presa in carico.
    /// </summary>
    /// <remarks>
    /// Va conservata e riusata da tutto quello che la Terra fa di sua iniziativa — il battito, la
    /// scadenza — altrimenti chi coordina non riconosce come propri gli esiti che gli tornano.
    /// </remarks>
    public Guid CorrelationId { get; set; }

    public DateTime DetectedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long ProjectionVersion { get; set; }
}
