using Evoluzione.IndependenceDay.Contracts.World;

namespace Evoluzione.IndependenceDay.Space.Domain.Services;

/// <summary>Quali navi porta un livello, nell'ordine esatto in cui partiranno.</summary>
public sealed record WavePlan(IReadOnlyList<ShipClass> Ships)
{
    public int Count => Ships.Count;

    /// <summary>Quanti colpi a segno servono per abbattere tutta l'ondata.</summary>
    public int Hits => Ships.Sum(Contracts.World.Ships.HitsToDestroy);
}

/// <summary>
/// La curva di difficolta': come cresce l'invasione livello dopo livello.
/// </summary>
/// <remarks>
/// E' la regola del gioco, e sta <b>qui e basta</b>: i numeri non si ripetono in nessuna
/// configurazione, perche' due copie divergono in silenzio e il gioco finisce per lanciare ondate
/// diverse da quelle su cui e' tarato il bilanciamento. Cresce su due assi — piu' navi, e navi piu'
/// pesanti — cosi' che a un certo punto non basti piu' rispondere a una minaccia alla volta.
/// <para>
/// Le munizioni <b>non</b> si ricaricano fra un livello e l'altro: quello che si spreca al livello
/// due non c'e' piu' al livello nove. E' li' che si decide quante ondate si resiste.
/// </para>
/// <para>
/// Il piano e' deterministico e ricostruibile da chiunque conosca il livello: il generatore lo
/// ricalcola invece di portarselo dietro, cosi' un riavvio non cambia l'ondata a meta'.
/// </para>
/// </remarks>
public sealed class WaveDifficulty
{
    /// <summary>
    /// Navi del primo livello: una per citta'.
    /// </summary>
    /// <remarks>
    /// Cinque e non due perche' la prima ondata deve poter fare male a <b>tutte</b> le citta'. Una
    /// Terra che non spara non deve cavarsela con tre citta' in piedi.
    /// </remarks>
    public int BaseShips { get; set; } = 5;

    /// <summary>Quante navi in piu' a ogni livello.</summary>
    public int ShipsPerLevel { get; set; } = 5;

    /// <summary>Da che livello compaiono gli incrociatori.</summary>
    public int CruisersFromLevel { get; set; } = 2;

    /// <summary>Da che livello compaiono le corazzate.</summary>
    public int BattleshipsFromLevel { get; set; } = 2;

    public WavePlan For(int level)
    {
        var l = Math.Max(1, level);
        var total = BaseShips + (l - 1) * ShipsPerLevel;

        var battleships = Math.Clamp(l - BattleshipsFromLevel + 1, 0, total);
        var cruisers = Math.Clamp(l - CruisersFromLevel + 1, 0, total - battleships);
        var fighters = total - battleships - cruisers;

        // Le piu' grosse per prime: chi arriva dopo trova i cannoni gia' impegnati, ed e' quello che
        // rende la coda dell'ondata il momento in cui si perde.
        var ships = new List<ShipClass>(total);
        ships.AddRange(Enumerable.Repeat(ShipClass.Battleship, battleships));
        ships.AddRange(Enumerable.Repeat(ShipClass.Cruiser, cruisers));
        ships.AddRange(Enumerable.Repeat(ShipClass.Fighter, fighters));

        return new WavePlan(ships);
    }
}
