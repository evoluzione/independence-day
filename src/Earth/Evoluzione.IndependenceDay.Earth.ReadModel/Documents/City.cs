using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Evoluzione.IndependenceDay.Earth.ReadModel.Documents;

/// <summary>Una citta' e il suo cannone: sono la stessa cosa, e cadono insieme.</summary>
public class City : IProjectionDocument
{
    [BsonId] public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int Integrity { get; set; }

    /// <summary>Colpi che restano a questo cannone per il resto della campagna.</summary>
    public int Rounds { get; set; }

    /// <summary>ready | firing | jammed | empty | lost</summary>
    public string Cannon { get; set; } = "ready";

    /// <summary>La nave a cui sta sparando, se sta sparando.</summary>
    public Guid Target { get; set; }

    /// <summary>Quando ha sparato l'ultima volta: e' da qui che la centrale di tiro conta la ricarica.</summary>
    public DateTime LastShotAt { get; set; }

    public DateTime UpdatedAt { get; set; }
    public long ProjectionVersion { get; set; }
}
