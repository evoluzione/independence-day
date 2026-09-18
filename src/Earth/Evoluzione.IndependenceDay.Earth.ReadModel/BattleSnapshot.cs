namespace Evoluzione.IndependenceDay.Earth.ReadModel;

public record CityView(
    Guid Id,
    string Name,
        int Integrity,
    bool Fallen,
        int Rounds,
        string Cannon,
        Guid Target);

public record EarthState(
    int CitiesStanding,
    int CitiesTotal,
        int RoundsLeft,
    IReadOnlyList<CityView> Cities);

public record AlienState(
    int ShipsInFlight,
    int ShipsDestroyed,
    int ShipsLanded);

public record LogView(Guid Id, DateTime At, string Step, string Detail, string CityName, string Tone);

public record ShipView(
    Guid Id,
    Guid CityId,
    string CityName,
        string Class,
    int Hits,
    int HitsToDestroy,
    string Status,
    DateTime DetectedAt,
        int MsToImpact,
        int CannonsFiring);

public record WaveSummary(
    int Wave,
    bool Won,
    string Verdict,
    DateTime StartedAt,
    DateTime EndedAt,
    int DurationSeconds,
    int ShipsLaunched,
    int ShipsDestroyed,
    int ShipsLanded,
        int RoundsSpent,
    int RoundsWasted,
        int RoundsLeft,
    int CitiesStanding,
    int CitiesTotal,
    bool GameOver,
        bool CampaignWon,
    IReadOnlyDictionary<string, int> Steps,
    IReadOnlyList<CityView> Cities);

public record InvasionView(
    int Wave,
    string Status,
    int TotalShips,
    int ShipsRemaining,
    bool Over,
        bool GameOver,
        bool CampaignWon,
    string Verdict,
    WaveSummary? Summary);

public record BattleSnapshot(
    EarthState Earth,
    AlienState Aliens,
    InvasionView Invasion,
    IReadOnlyDictionary<string, int> Steps,
    IReadOnlyList<ShipView> Ships,
    IReadOnlyList<LogView> Log);
