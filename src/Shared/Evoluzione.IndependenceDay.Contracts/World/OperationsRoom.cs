namespace Evoluzione.IndependenceDay.Contracts.World;

/// <summary>
/// La sala operativa: quante intercettazioni si riescono a seguire insieme.
/// </summary>
/// <remarks>
/// Un processo aperto occupa una linea, e la libera quando si chiude. Sono piu' delle navi che possono
/// piu' delle navi che possono essere in volo insieme — quindi non e' una risorsa da dosare: e' la
/// ragione per cui <b>chiudere</b> un processo e' una mossa e non una formalita'.
/// <para>
/// Un processo che non si chiude mai non fa niente di male subito. Tiene la sua linea, e basta. Ma le
/// navi passano a decine, le linee non tornano indietro, e a un certo punto una nave viene avvistata
/// e non c'e' nessuno che la prenda in carico: nessun ordine, nessun cannone, nessun errore. Solo una
/// citta' in meno.
/// <para>
/// Le linee tornano tutte libere quando comincia una campagna <b>nuova</b>, come le citta'. Dentro la
/// stessa campagna non torna indietro niente.
/// </para>
/// </para>
/// </remarks>
public static class OperationsRoom
{
    /// <summary>Quante intercettazioni possono essere aperte nello stesso momento.</summary>
    public const int Lines = 16;
}
