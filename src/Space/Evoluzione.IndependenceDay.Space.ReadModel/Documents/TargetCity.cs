using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Evoluzione.IndependenceDay.Space.ReadModel.Documents;

/// <summary>
/// Una citta' vista dallo Spazio: gli interessa una cosa sola, se e' ancora in piedi.
/// </summary>
/// <remarks>
/// Lo Spazio non conosce il modello della Terra: sa che una citta' e' caduta perche' l'ha sentito
/// sul bus. Gli serve per non mandare navi su macerie.
/// </remarks>
public class TargetCity : IProjectionDocument
{
    [BsonId] public Guid Id { get; set; }
    public bool Standing { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long ProjectionVersion { get; set; }
}
