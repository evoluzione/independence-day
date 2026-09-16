namespace Evoluzione.IndependenceDay.Contracts.World;

/// <summary>
/// L'invasione: la nave madre e gli identificativi fissi che i servizi condividono.
/// </summary>
/// <remarks>
/// L'<b>andamento</b> dell'invasione e' un aggregato dello Spazio — le ondate cominciano, finiscono, e
/// non possono cominciare due volte. Qui stanno solo le costanti.
/// </remarks>
public static class Invasion
{
    /// <summary>L'aggregato dell'invasione. Uno solo, con un id fisso: le ondate sono le sue.</summary>
    public static readonly Guid Id = Guid.Parse("99999999-0000-0000-0000-000000000000");

    public static readonly Guid MotherShipId = Guid.Parse("99999999-0000-0000-0000-000000000001");

    /// <summary>
    /// L'ultimo livello: superato quello, la Terra ha vinto e la campagna finisce.
    /// </summary>
    /// <remarks>
    /// Una partita deve avere una fine raggiungibile, altrimenti "quante ondate hai resistito" non e'
    /// un punteggio ma una misura di pazienza. Dieci livelli con la curva di difficolta' corrente
    /// chiedono quasi tutta la capacita' di fuoco delle cinque citta': si vince solo allocando bene.
    /// <para>
    /// Lo conoscono tutti e due i servizi — lo Spazio per non mandare un'undicesima ondata, la Terra
    /// per sapere quando dichiarare vinta la campagna — quindi sta qui e non nella configurazione di
    /// uno dei due.
    /// </para>
    /// </remarks>
    public const int LastLevel = 10;

    /// <summary>
    /// Quanto tempo ha la difesa per fermare una flotta prima che tocchi terra.
    /// </summary>
    /// <remarks>
    /// Sta qui e non solo nella configurazione della Terra perche' e' una regola del mondo, non un
    /// dettaglio di un servizio: chi scrive una saga deve poterci contare per decidere se una forza
    /// fa in tempo ad arrivare. Otto secondi contro cento millisecondi a unita' vuol dire ottanta
    /// unita', non una di piu'.
    /// </remarks>
    public const int ApproachSeconds = 8;
}
