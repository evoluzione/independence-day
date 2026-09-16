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
/// I guasti sono <b>deterministici</b>: nessun dado, nessuna probabilita' nascosta. Sono scritti qui
/// in chiaro perche' la sfida e' gestirli, non indovinarli. L'altro guasto — l'ordine che non arriva
/// — non e' dei cannoni: vive sul collegamento, in <see cref="Radio"/>.
/// </para>
/// </remarks>
public static class Armory
{
    /// <summary>
    /// Colpi in dotazione a ogni citta' per <b>tutta</b> la campagna. Non si ricaricano mai.
    /// </summary>
    public const int RoundsPerCity = 40;

    /// <summary>Quanto passa fra un colpo e il successivo dello stesso cannone.</summary>
    public const int ReloadMs = 350;

    /// <summary>
    /// Ogni quanti colpi uno manca il bersaglio.
    /// </summary>
    /// <remarks>
    /// Un cannone contraerea che centra sempre non e' un cannone contraerea. Il colpo parte, consuma
    /// la munizione e non fa danno: si ricarica e si riprova, ed e' il cannone stesso a farlo, perche'
    /// il fuoco resta aperto. Chi coordina non deve fare niente — ma il conto dei colpi che servono
    /// a portare giu' una nave cresce, e cresce anche il tempo per cui quel cannone resta occupato.
    /// </remarks>
    public const int MissEveryShots = 3;

    /// <summary>Se il colpo numero <paramref name="shot"/> di quel cannone manca il bersaglio.</summary>
    public static bool Misses(int shot) => shot % MissEveryShots == 0;

    /// <summary>
    /// Ogni quanti colpi un cannone si inceppa.
    /// </summary>
    /// <remarks>
    /// Il colpo che inceppa non parte e non consuma munizioni: costa il tempo di accorgersene, di
    /// mandare la riparazione e di riaprire il fuoco. Un cannone inceppato e dimenticato e' perso per
    /// il resto della campagna.
    /// </remarks>
    public const int JamEveryShots = 10;

    /// <summary>Quanto costa in munizioni <b>ogni tentativo</b> di rimettere in sesto un cannone.</summary>
    public const int RepairCost = 3;

    /// <summary>
    /// Ogni quante riparazioni una non prende.
    /// </summary>
    /// <remarks>
    /// Un guasto vero non si aggiusta sempre al primo colpo. Il tentativo consuma i suoi colpi e il
    /// cannone resta inceppato — e la Terra lo racconta, con <c>CannonStillJammed</c>: e' un evento,
    /// non un silenzio, e va ripetuto.
    /// <para>
    /// Il conto e' sulle riparazioni di <b>quel</b> cannone, riuscite e fallite insieme: la prima
    /// prende, la seconda no, e si ricomincia. Cosi' il primo inceppamento della campagna si risolve
    /// al primo tentativo, e insistere diventa necessario un livello piu' in la'. Due tentativi di
    /// fila non falliscono mai: riprovare basta sempre.
    /// </para>
    /// </remarks>
    public const int RepairFailsEvery = 2;

    /// <summary>
    /// Se il tentativo numero <paramref name="repair"/> su quel cannone rimette a posto il guasto.
    /// </summary>
    public static bool Repaired(int repair) => repair % RepairFailsEvery != 0;
}
