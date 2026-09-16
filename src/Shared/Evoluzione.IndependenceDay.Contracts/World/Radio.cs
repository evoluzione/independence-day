namespace Evoluzione.IndependenceDay.Contracts.World;

/// <summary>
/// Il collegamento fra chi coordina la difesa e la Terra. Non e' affidabile.
/// </summary>
/// <remarks>
/// Un ordine su venticinque non arriva a destinazione. Si perde <b>per strada</b>, prima di raggiungere
/// qualunque cannone: sulla Terra non succede niente, quindi non c'e' niente da raccontare e non
/// torna indietro nessun evento — nemmeno un rifiuto. C'e' solo silenzio.
/// <para>
/// E' importante che sia cosi' e non un evento di errore. Un errore che torna indietro si gestisce
/// guardandolo, ed e' il caso facile; il caso che rompe i processi distribuiti e' l'altro, la
/// risposta che non arriva mai. Un aggregato che ricevesse il comando e decidesse di ignorarlo
/// sarebbe una rete che finge: un aggregato non scarta mai un ordine che ha ancora senso, e quando
/// esce in silenzio — una riconsegna, un ordine diventato inutile — e' idempotenza, non un guasto.
/// </para>
/// <para>
/// La perdita e' deterministica — nessun dado, nessuna probabilita' nascosta — e il contatore avanza
/// a ogni ordine, perso o consegnato. Quindi <b>due ordini di fila non si perdono mai</b>: riprovare
/// basta sempre, e non puo' avvitarsi in un giro infinito.
/// </para>
/// </remarks>
public static class Radio
{
    /// <summary>
    /// Ogni quanti ordini se ne perde uno.
    /// </summary>
    /// <remarks>
    /// Il conto e' su <b>tutti</b> gli ordini spediti, ritentativi compresi — e chi coordina ne
    /// spedisce parecchi, perche' insiste a ogni battito finche' una nave non ha un cannone addosso.
    /// Con un ordine su tredici la perdita si concentrava proprio quando il sistema era sotto
    /// pressione, e gli ultimi livelli diventavano ingiocabili. Uno su venticinque lascia la sfida
    /// dov'era senza renderla una punizione per aver insistito.
    /// </remarks>
    public const int LostEveryOrders = 25;

    /// <summary>
    /// Se l'ordine numero <paramref name="order"/> arriva a destinazione.
    /// </summary>
    /// <remarks>
    /// Il resto e' sedici e non zero per una ragione di percorso: i primi ordini di una partita devono
    /// arrivare tutti. Vedere sparire il primo comando in assoluto sembra un guasto del gioco, non una
    /// regola del gioco — e i primi due livelli si superano senza sapere che gli ordini si perdono.
    /// </remarks>
    public static bool Delivers(int order) => order % LostEveryOrders != 16;
}
