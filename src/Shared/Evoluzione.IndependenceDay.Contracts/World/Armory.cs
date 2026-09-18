namespace Evoluzione.IndependenceDay.Contracts.World;

public static class Armory
{
    public const int RoundsPerCity = 40;

    public const int ReloadMs = 350;

    public const int MissEveryShots = 3;

    public static bool Misses(int shot) => shot % MissEveryShots == 0;

    public const int JamEveryShots = 10;

    public const int RepairCost = 3;

    public const int ResupplyRounds = 20;

    public const int ResupplySeconds = 3;
}
