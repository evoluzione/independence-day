namespace Evoluzione.IndependenceDay.Contracts.World;

public enum ShipClass
{
    Fighter = 0,
    Cruiser = 1,
    Battleship = 2
}

public static class Ships
{
    public static int HitsToDestroy(ShipClass stazza) => stazza switch
    {
        ShipClass.Fighter => 1,
        ShipClass.Cruiser => 4,
        ShipClass.Battleship => 9,
        _ => 1
    };

    public static int DamageOf(ShipClass stazza) => stazza switch
    {
        ShipClass.Fighter => 100,
        ShipClass.Cruiser => 100,
        ShipClass.Battleship => 100,
        _ => 0
    };

    public static string NameOf(ShipClass stazza) => stazza switch
    {
        ShipClass.Fighter => "caccia",
        ShipClass.Cruiser => "incrociatore",
        ShipClass.Battleship => "corazzata",
        _ => "nave"
    };

    public const int FullIntegrity = 100;
}
