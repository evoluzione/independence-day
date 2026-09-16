namespace Evoluzione.IndependenceDay.Contracts.World;

/// <summary>
/// I cannoni della Terra: quanto hanno, quanto in fretta sparano, e come si rompono.
/// </summary>
/// <remarks>
/// Una citta', un cannone. Un cannone spara a <b>una nave alla volta</b> e continua finche' non gli
/// si dice di smettere: e' questo che rende il cessate il fuoco una mossa e non una formalita'. Un
/// cannone lasciato a sparare su una nave gia' abbattuta non e' soltanto munizione buttata, e' un
/// cannone che non c'e' quando arriva la nave dopo.
/// <para>
/// I guasti sono <b>deterministici</b>. Nessun dado: due squadre diverse affrontano gli stessi
/// inceppamenti e gli stessi ordini persi, allo stesso momento della campagna. Sono scritti qui in
/// chiaro perche' la sfida e' gestirli, non indovinarli.
/// </para>
/// </remarks>
public static class Armory
{
    /// <summary>
    /// Colpi in dotazione a ogni citta' per <b>tutta</b> la campagna. Non si ricaricano mai.
    /// </summary>
    public const int RoundsPerCity = 100;

    /// <summary>Quanto passa fra un colpo e il successivo dello stesso cannone.</summary>
    public const int ReloadMs = 400;

    /// <summary>
    /// Ogni quanti colpi un cannone si inceppa.
    /// </summary>
    /// <remarks>
    /// Il colpo che inceppa non parte e non consuma munizioni: costa il tempo di accorgersene, di
    /// mandare la riparazione e di riaprire il fuoco. Un cannone inceppato e dimenticato e' perso per
    /// il resto della campagna.
    /// </remarks>
    public const int JamEveryShots = 9;

    /// <summary>Quanto costa in munizioni rimettere in sesto un cannone inceppato.</summary>
    public const int RepairCost = 3;

    /// <summary>
    /// Se un ordine si perde per strada.
    /// </summary>
    /// <remarks>
    /// Un ordine perso non produce <b>nessun</b> evento: non c'e' un errore da intercettare, c'e' solo
    /// silenzio. Chi coordina se ne accorge soltanto perche' al battito successivo la nave e' ancora
    /// viva e nessuno le sta sparando.
    /// <para>
    /// Il conto dei tentativi cresce a ogni ordine eseguito o perso: due ordini di fila non si perdono
    /// mai, quindi riprovare basta sempre. Insistere e' la risposta giusta, e non puo' avvitarsi.
    /// <para>
    /// Uno su tredici e non uno su sette: una nave che tocca terra rade al suolo la citta', quindi
    /// una difesa condotta bene deve poterle fermare <b>tutte</b>. Con guasti piu' fitti la perfezione
    /// diventava impossibile, e un gioco che si perde comunque non insegna a giocarlo meglio.
    /// </para>
    /// </para>
    /// </remarks>
    public static bool OrderLost(int ordersReceived) => ordersReceived % 13 == 3;
}
