using MongoDB.Bson.Serialization.Attributes;

namespace Evoluzione.IndependenceDay.Earth.ReadModel.Documents;

public class BattleLogEntry
{
    [BsonId] public Guid Id { get; set; }
    public DateTime At { get; set; }

    public string Step { get; set; } = string.Empty;

    public string Detail { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public Guid ShipId { get; set; }

    public int Wave { get; set; }

    public string Tone { get; set; } = "ok";

    public int Amount { get; set; }
}
