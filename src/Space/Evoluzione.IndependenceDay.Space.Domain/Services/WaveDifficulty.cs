using Evoluzione.IndependenceDay.Contracts.World;

namespace Evoluzione.IndependenceDay.Space.Domain.Services;

public sealed record WavePlan(IReadOnlyList<ShipClass> Ships)
{
    public int Count => Ships.Count;

    public int Hits => Ships.Sum(Contracts.World.Ships.HitsToDestroy);
}

public sealed class WaveDifficulty
{
    public int Fighters { get; set; } = 12;

    public int Cruisers { get; set; } = 18;

    public int Battleships { get; set; } = 6;

    public WavePlan Plan()
    {
        var perBlock = Gcd(Gcd(Battleships, Cruisers), Fighters);
        var blocks = Math.Max(1, perBlock);

        var ships = new List<ShipClass>(Fighters + Cruisers + Battleships);
        for (var b = 0; b < blocks; b++)
        {

            ships.AddRange(Enumerable.Repeat(ShipClass.Battleship, Battleships / blocks));
            ships.AddRange(Enumerable.Repeat(ShipClass.Cruiser, Cruisers / blocks));
            ships.AddRange(Enumerable.Repeat(ShipClass.Fighter, Fighters / blocks));
        }

        return new WavePlan(ships);
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
