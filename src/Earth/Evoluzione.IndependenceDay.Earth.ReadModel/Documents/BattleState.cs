using MongoDB.Bson.Serialization.Attributes;

namespace Evoluzione.IndependenceDay.Earth.ReadModel.Documents;

/// <summary>
/// A che punto e' la battaglia, dal lato della Terra: quale ondata, e se ne e' in corso una.
/// </summary>
/// <remarks>
/// Documento unico. Non e' una copia di quello dello Spazio: e' quello che la Terra ha capito dagli
/// eventi sul bus, ed e' la chiave con cui navi e diario vengono filtrati sull'ondata giusta.
/// </remarks>
public class BattleState
{
    public const string Key = "battle";

    [BsonId] public string Id { get; set; } = Key;

    /// <summary>Progressivo di sempre: la chiave con cui navi e diario si filtrano.</summary>
    public int Wave { get; set; }

    /// <summary>Posizione nella campagna, e quindi la difficolta'. E' il punteggio.</summary>
    public int Level { get; set; }

    public int Ships { get; set; }
    public bool Running { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
