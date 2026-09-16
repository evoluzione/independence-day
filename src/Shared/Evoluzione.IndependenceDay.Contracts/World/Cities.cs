namespace Evoluzione.IndependenceDay.Contracts.World;

/// <summary>
/// La mappa del mondo: le citta' che lo Spazio puo' prendere di mira e che la Terra difende.
/// </summary>
/// <remarks>
/// Gli identificativi sono fissi e scritti a mano, non generati. Sono la stessa citta' su due servizi
/// che non si parlano se non via bus: se ognuno se li generasse, lo Spazio attaccherebbe citta' che
/// la Terra non ha mai messo in difesa, e il comando finirebbe su un aggregato inesistente.
/// Essendo fissi, sopravvivono anche al riavvio: gli stream su EventStore restano gli stessi.
/// </remarks>
public static class Cities
{
    /// <summary>
    /// La difesa della Terra e' un aggregato solo, con un id fisso.
    /// </summary>
    /// <remarks>
    /// Non uno per citta': la riserva e' unica, quindi "non puoi piazzare piu' di quello che hai" e'
    /// un invariante che attraversa tutte e cinque. Un aggregato e' il confine di un invariante, e
    /// questo invariante e' globale.
    /// </remarks>
    public static readonly Guid DefenseId = Guid.Parse("22222222-0000-0000-0000-000000000000");

    public static readonly IReadOnlyList<(Guid Id, string Name)> All =
    [
        (Guid.Parse("11111111-0000-0000-0000-000000000001"), "New York"),
        (Guid.Parse("11111111-0000-0000-0000-000000000002"), "Los Angeles"),
        (Guid.Parse("11111111-0000-0000-0000-000000000003"), "Washington D.C."),
        (Guid.Parse("11111111-0000-0000-0000-000000000004"), "Houston"),
        (Guid.Parse("11111111-0000-0000-0000-000000000005"), "Area 51")
    ];

    /// <summary>
    /// Il nome della citta', o vuoto se l'id non e' di nessuna.
    /// </summary>
    /// <remarks>
    /// Guid.Empty vale "nessuna citta'": lo usano le righe di diario che riguardano l'ondata intera e
    /// non una citta' sola. Tornare l'id come nome le faceva comparire con un GUID di zeri accanto.
    /// </remarks>
    public static string NameOf(Guid id) =>
        All.FirstOrDefault(city => city.Id == id).Name ?? string.Empty;
}
