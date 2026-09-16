namespace Evoluzione.IndependenceDay.Earth.ReadModel;

/// <summary>Una citta' e il suo cannone, come li vede la pagina.</summary>
public record CityView(
    Guid Id,
    string Name,
    /// <summary>Da 100 a 0: la barra verde sotto la citta'.</summary>
    int Integrity,
    bool Fallen,
    /// <summary>Colpi che restano per il resto della campagna.</summary>
    int Rounds,
    /// <summary>ready | firing | jammed | empty | lost</summary>
    string Cannon,
    /// <summary>La nave a cui sta sparando, se sta sparando.</summary>
    Guid Target);

public record EarthState(
    int CitiesStanding,
    int CitiesTotal,
    /// <summary>Colpi che restano in tutto: e' la risorsa che decide quanto si va avanti.</summary>
    int RoundsLeft,
    IReadOnlyList<CityView> Cities);

public record AlienState(
    int ShipsInFlight,
    int ShipsDestroyed,
    int ShipsLanded);

public record LogView(Guid Id, DateTime At, string Step, string Detail, string CityName, string Tone);

/// <summary>Una nave come la vede la pagina: dove punta, quanto ha incassato, quanto le resta.</summary>
public record ShipView(
    Guid Id,
    Guid CityId,
    string CityName,
    /// <summary>caccia | incrociatore | corazzata</summary>
    string Class,
    int Hits,
    int HitsToDestroy,
    string Status,
    DateTime DetectedAt,
    /// <summary>Quanto manca all'impatto, in millisecondi. La pagina lo scala da sola.</summary>
    int MsToImpact,
    /// <summary>Quanti cannoni le stanno sparando adesso.</summary>
    int CannonsFiring);

/// <summary>Il resoconto di un'ondata conclusa. Si calcola dal vivo, prima di rimettere in linea le citta'.</summary>
public record WaveSummary(
    int Wave,
    int Level,
    bool Won,
    string Verdict,
    DateTime StartedAt,
    DateTime EndedAt,
    int DurationSeconds,
    int ShipsLaunched,
    int ShipsDestroyed,
    int ShipsLanded,
    /// <summary>Colpi sparati nell'ondata, e quanti sono andati nel vuoto.</summary>
    int RoundsSpent,
    int RoundsWasted,
    /// <summary>Colpi che restano in tutto: e' quello che si porta all'ondata dopo.</summary>
    int RoundsLeft,
    int CitiesStanding,
    int CitiesTotal,
    bool GameOver,
    /// <summary>Superato l'ultimo livello: la campagna e' vinta.</summary>
    bool CampaignWon,
    IReadOnlyDictionary<string, int> Steps,
    IReadOnlyList<CityView> Cities);

/// <summary>
/// Lo stato dell'invasione per la pagina: <c>idle</c> prima di cominciare, <c>running</c> durante,
/// <c>over</c> quando l'ondata e' chiusa e resta da guardare il resoconto.
/// </summary>
public record InvasionView(
    int Wave,
    int Level,
    string Status,
    int TotalShips,
    int ShipsRemaining,
    bool Over,
    /// <summary>Non c'e' piu' una citta' da difendere: la campagna e' finita qui.</summary>
    bool GameOver,
    /// <summary>Superato l'ultimo livello con almeno una citta' in piedi: vinta.</summary>
    bool CampaignWon,
    /// <summary>L'ultimo livello della campagna: il traguardo.</summary>
    int LastLevel,
    string Verdict,
    WaveSummary? Summary);

public record BattleSnapshot(
    EarthState Earth,
    AlienState Aliens,
    InvasionView Invasion,
    IReadOnlyDictionary<string, int> Steps,
    IReadOnlyList<ShipView> Ships,
    IReadOnlyList<LogView> Log);
