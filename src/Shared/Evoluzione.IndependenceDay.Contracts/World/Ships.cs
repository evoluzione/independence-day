namespace Evoluzione.IndependenceDay.Contracts.World;

/// <summary>Le tre stazze aliene, dalla piu' leggera alla piu' pesante.</summary>
public enum ShipClass
{
    Fighter = 0,
    Cruiser = 1,
    Battleship = 2
}

/// <summary>
/// Quanto e' dura una nave e quanto costa lasciarla arrivare.
/// </summary>
/// <remarks>
/// Quanti colpi servano ad abbatterne una <b>lo sa solo la Terra</b>. Chi coordina la difesa non lo
/// legge da nessuna parte: spara, e aspetta di sentirsi dire che e' caduta. E' una scelta di
/// progetto, non una dimenticanza — il conto di quanti colpi mancano e' una decisione di dominio, e
/// il dominio e' qui.
/// </remarks>
public static class Ships
{
    /// <summary>Quanti colpi a segno servono per portarla giu'.</summary>
    public static int HitsToDestroy(ShipClass stazza) => stazza switch
    {
        ShipClass.Fighter => 1,
        ShipClass.Cruiser => 4,
        ShipClass.Battleship => 9,
        _ => 1
    };

    /// <summary>
    /// Il danno che fa toccando terra, su cento di integrita'.
    /// </summary>
    /// <remarks>
    /// Cento per tutte le stazze: <b>una nave che tocca terra rade al suolo la citta'</b>, e con lei
    /// il suo cannone. Non e' una gradazione mancata, e' la regola — ed e' quello che rende il non
    /// fare niente una sconfitta immediata invece di un costo da ammortizzare.
    /// <para>
    /// Il prezzo non e' un punto: e' un quinto della potenza di fuoco per tutto il resto della
    /// campagna. Nessuna nave e' trascurabile, nemmeno un caccia al primo livello.
    /// </para>
    /// </remarks>
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
