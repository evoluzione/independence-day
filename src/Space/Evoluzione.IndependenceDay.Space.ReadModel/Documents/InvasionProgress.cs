using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using MongoDB.Bson.Serialization.Attributes;

namespace Evoluzione.IndependenceDay.Space.ReadModel.Documents;

/// <summary>
/// Lo stato dell'invasione come lo legge il generatore: quale ondata, quante navi porta, e se
/// deve ancora lanciare.
/// </summary>
/// <remarks>
/// Documento unico, con l'id fisso dell'aggregato. E' una proiezione e non uno stato in memoria
/// perche' il generatore deve ritrovare l'ondata in corso dopo un riavvio del servizio.
/// </remarks>
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
