using MongoDB.Bson.Serialization.Attributes;

namespace Evoluzione.IndependenceDay.Earth.ReadModel.Documents;

public class BattleState
{
    public const string Key = "battle";

    [BsonId] public string Id { get; set; } = Key;

    public int Wave { get; set; }

    public int Ships { get; set; }
    public bool Running { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
