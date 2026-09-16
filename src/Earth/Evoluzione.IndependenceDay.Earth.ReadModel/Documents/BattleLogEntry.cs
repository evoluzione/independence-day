using MongoDB.Bson.Serialization.Attributes;

namespace Evoluzione.IndependenceDay.Earth.ReadModel.Documents;

public class BattleLogEntry
{
    [BsonId] public Guid Id { get; set; }
    public DateTime At { get; set; }

    /// <summary>Il passo o l'esito: fire, shot, destroyed, jammed, wasted, landed...</summary>
    public string Step { get; set; } = string.Empty;

    public string Detail { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public Guid ShipId { get; set; }

    /// <summary>L'ondata a cui appartiene.</summary>
    public int Wave { get; set; }

    /// <summary>ok | warn | bad — serve solo al colore della riga.</summary>
    public string Tone { get; set; } = "ok";

    /// <summary>Quante unita' riguarda la riga, quando ne riguarda. Serve a sommarle nel resoconto.</summary>
    public int Amount { get; set; }
}
