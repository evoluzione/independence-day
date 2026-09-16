using Evoluzione.IndependenceDay.Contracts.World;
using Muflone.Core;

namespace Evoluzione.IndependenceDay.Earth.Domain.Entities;

/// <summary>In che stato e' un cannone.</summary>
public enum CannonStatus
{
    /// <summary>Fermo e disponibile: e' questo che serve per poter aprire il fuoco.</summary>
    Ready = 0,

    /// <summary>Sta sparando a una nave, e continuera' finche' non gli si dice di smettere.</summary>
    Firing = 1,

    /// <summary>Inceppato: non spara a nessuno e non si sblocca da solo.</summary>
    Jammed = 2,

    /// <summary>Senza munizioni. Non si ricarica: e' fuori dalla campagna.</summary>
    Empty = 3,

    /// <summary>La citta' e' caduta e il cannone con lei.</summary>
    Lost = 4
}

/// <summary>Il cannone di una citta'. Uno solo, e spara a una nave alla volta.</summary>
public sealed class Cannon
{
    public string Name { get; set; } = string.Empty;
    public int Integrity { get; set; }
    public int Rounds { get; set; }
    public CannonStatus Status { get; set; }

    /// <summary>La nave a cui sta sparando, se sta sparando.</summary>
    public string? Target { get; set; }

    /// <summary>
    /// Quante volte si e' premuto il grilletto, colpi partiti e inceppamenti insieme.
    /// </summary>
    /// <remarks>
    /// Conta anche i colpi che non sono partiti, altrimenti un cannone appena riparato si
    /// incepperebbe di nuovo al primo tentativo, e poi ancora, per sempre.
    /// </remarks>
    public int Attempts { get; set; }

    /// <summary>Quante volte si e' provato a ripararlo, riuscite e fallite insieme.</summary>
    public int Repairs { get; set; }

    /// <summary>
    /// La nave su cui si e' inceppato, se e' inceppato.
    /// </summary>
    /// <remarks>
    /// Non e' <see cref="Target"/>: un cannone inceppato non spara a nessuno. Serve a sapere a chi
    /// raccontare che e' ancora fermo — al processo che quel cannone lo ha in mano.
    /// </remarks>
    public string? JammedOn { get; set; }

    public bool Fallen => Integrity <= 0;

    /// <summary>Puo' ricevere un ordine di fuoco: fermo, intero e con qualcosa da sparare.</summary>
    public bool Available => Status == CannonStatus.Ready && Rounds > 0;
}

/// <summary>Una nave aliena in avvicinamento, e quanto ha gia' incassato.</summary>
public sealed record Incoming(string CityId, ShipClass Class, int Hits);

/// <summary>
/// La difesa della Terra: cinque citta', un cannone ciascuna, e le navi in arrivo.
/// </summary>
/// <remarks>
/// Un aggregato solo e non uno per citta'. I cannoni sono cinque ma la domanda «ce n'e' uno libero?»
/// li attraversa tutti, e un aggregato e' il confine di un invariante.
/// <para>
/// Le regole sono quattro, e si vedono tutte a schermo:
/// </para>
/// <list type="number">
/// <item>Un cannone spara a <b>una nave alla volta</b>, e quale cannone tocchi lo sceglie la Terra:
/// e' lei a sapere chi e' libero, chi e' rotto e a chi restano colpi. Un colpo su tre manca il bersaglio:
/// la munizione se ne va e la nave regge.</item>
/// <item>Un cannone a secco resta <b>assegnato</b> alla sua nave finche' non lo si restituisce, e un
/// cannone inceppato resta inceppato finche' non lo si ripara — e la riparazione non sempre prende.</item>
/// <item>Il fuoco, una volta aperto, <b>non si ferma da solo</b>. Nemmeno quando la nave e' caduta:
/// il cannone continua a sparare su relitti finche' non arriva un cessate il fuoco.</item>
/// <item>Ogni tanto un cannone si inceppa. Non e' un errore del chiamante, e' un fatto della
/// battaglia: si risponde con un evento e si va avanti.</item>
/// <item>Quando il tempo scade la nave tocca terra, e se la citta' cade perde anche il cannone.</item>
/// </list>
/// <para>
/// Nessuna regola solleva un'eccezione: un ordine impossibile e' un evento, non un errore. E nessun
/// ordine che abbia ancora senso resta senza risposta — si esce in silenzio solo davanti a una
/// riconsegna o a un ordine diventato inutile, che e' idempotenza. Gli ordini che si <b>perdono</b>
/// non arrivano mai fin qui: si fermano sul collegamento, e questo aggregato non sa che esistano.
/// </para>
/// </remarks>
public class EarthDefense : AggregateRoot
{
    protected EarthDefense()
    {
    }

    protected EarthDefense(EarthId id, int rounds, int integrity, Guid correlationId)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(integrity);

        if (correlationId == Guid.Empty)
            throw new ArgumentException("CorrelationId cannot be empty.", nameof(correlationId));

        RaiseEvent(new EarthCommissioned(id, rounds, correlationId));

        // Qualificato: dentro questo aggregato "Cities" e' la sua mappa delle citta', non la
        // costante condivisa.
        foreach (var city in Contracts.World.Cities.All)
            RaiseEvent(new EarthCityCommissioned(id, new CityId(city.Id), city.Name, integrity, rounds,
                correlationId));
    }

    public Dictionary<string, Cannon> Cannons { get; private set; } = [];
    public Dictionary<string, Incoming> Ships { get; private set; } = [];
    public int LastRecommissionedWave { get; private set; }

    public static EarthDefense Commission(EarthId id, int rounds, int integrity, Guid correlationId) =>
        new(id, rounds, integrity, correlationId);

    public void Recommission(int wave, int rounds, int integrity, Guid correlationId)
    {
        // Gia' rimessa in piedi per questa ondata: e' una riconsegna, non un secondo ripristino.
        if (wave == LastRecommissionedWave)
            return;

        RaiseEvent(new EarthRecommissioned((EarthId)Id, rounds, wave, integrity, correlationId));
    }

    /// <summary>Prende in carico una nave avvistata: da adesso si accettano ordini di fuoco su di lei.</summary>
    public void DetectShip(CityId cityId, ShipId shipId, ShipClass shipClass, Guid correlationId)
    {
        // Riconsegna dell'avvistamento, o nave gia' risolta: non si riapre una partita chiusa.
        if (Ships.ContainsKey(shipId.Value) || !Cannons.TryGetValue(cityId.Value, out var city) || city.Fallen)
            return;

        RaiseEvent(new EarthShipDetected((EarthId)Id, cityId, shipId, shipClass, correlationId));
    }

    /// <summary>
    /// Apre il fuoco su una nave con il cannone che ha meno colpi fra quelli liberi.
    /// </summary>
    /// <remarks>
    /// La scelta del cannone e' della Terra e non di chi ordina, perche' e' la Terra a saperne lo
    /// stato. Prende il piu' <b>scarico</b> fra quelli liberi: si consumano prima le riserve piccole
    /// e si tengono indietro quelle piene, cosi' la potenza di fuoco che resta e' concentrata invece
    /// che spalmata su cinque cannoni tutti quasi a secco.
    /// <para>
    /// Ha un prezzo, ed e' voluto: un cannone quasi finito puo' esaurirsi <b>in mezzo</b> a una nave.
    /// Non e' un caso limite di fine partita, e' un fatto normale da meta' campagna in poi.
    /// </para>
    /// </remarks>
    public void OpenFire(ShipId shipId, Guid correlationId)
    {
        // Nave sconosciuta: gia' abbattuta, gia' atterrata, o un ordine arrivato prima dell'avvistamento.
        if (!Ships.ContainsKey(shipId.Value))
            return;

        // Il piu' scarico fra i liberi, a parita' il primo in ordine di identificativo: nessun
        // sorteggio, cosi' due partite con le stesse mosse fanno le stesse scelte.
        var chosen = Cannons
            .Where(entry => entry.Value.Available)
            .OrderBy(entry => entry.Value.Rounds)
            .ThenBy(entry => entry.Key, StringComparer.Ordinal)
            .Select(entry => (Id: entry.Key, Cannon: entry.Value))
            .FirstOrDefault();

        if (chosen.Cannon is null)
        {
            RaiseEvent(new EarthNoCannonReady((EarthId)Id, shipId, correlationId));
            return;
        }

        var city = new CityId(Guid.Parse(chosen.Id));

        RaiseEvent(new EarthFireOpened((EarthId)Id, city, shipId, chosen.Cannon.Rounds, correlationId));
    }

    /// <summary>Quel cannone smette di sparare a quella nave e torna disponibile.</summary>
    /// <remarks>
    /// Il bersaglio fa parte dell'ordine: un cessate il fuoco in ritardo non deve spegnere un cannone
    /// che nel frattempo e' stato messo su un'altra nave.
    /// </remarks>
    public void CeaseFire(CityId cityId, ShipId shipId, Guid correlationId)
    {
        // Vale anche per un cannone a secco: finche' ha un bersaglio risulta impegnato su quella nave,
        // e restituirlo e' l'unico modo di liberarlo.
        if (!Cannons.TryGetValue(cityId.Value, out var cannon) ||
            cannon.Status is not (CannonStatus.Firing or CannonStatus.Empty) ||
            cannon.Target != shipId.Value)
            return;

        RaiseEvent(new EarthFireCeased((EarthId)Id, cityId, shipId, cannon.Rounds, correlationId));
    }

    /// <summary>
    /// Prova a rimettere in sesto un cannone inceppato. Costa qualche colpo e non riapre il fuoco.
    /// </summary>
    /// <remarks>
    /// Il tentativo puo' <b>non prendere</b>. Non e' un rifiuto e non e' un errore: i colpi se ne
    /// vanno lo stesso, il cannone resta inceppato, e la Terra lo racconta. Chi ha ordinato la
    /// riparazione lo scopre da li', e insiste.
    /// </remarks>
    public void RepairCannon(CityId cityId, ShipId shipId, Guid correlationId)
    {
        if (!Cannons.TryGetValue(cityId.Value, out var cannon) || cannon.Status != CannonStatus.Jammed)
            return;

        var left = Math.Max(0, cannon.Rounds - Armory.RepairCost);

        if (Armory.Repaired(cannon.Repairs + 1))
            RaiseEvent(new EarthCannonRepaired((EarthId)Id, cityId, shipId, left, correlationId));
        else
            RaiseEvent(new EarthCannonStillJammed((EarthId)Id, cityId, shipId, left, correlationId));

        if (left <= 0)
            RaiseEvent(new EarthCannonEmpty((EarthId)Id, cityId, shipId, correlationId));
    }

    /// <summary>
    /// Un colpo.
    /// </summary>
    /// <remarks>
    /// La cadenza non e' qui: la tiene la centrale di tiro, che e' quella che preme il grilletto.
    /// Il tempo che una cosa impiega a succedere lo conosce chi la esegue, come per il cronometro
    /// dell'avvicinamento.
    /// <para>
    /// Il bersaglio non viene ricontrollato prima di sparare: se non c'e' piu', il colpo parte lo
    /// stesso e si perde. E' il prezzo di un cessate il fuoco che non e' arrivato, e deve vedersi.
    /// </para>
    /// </remarks>
    public void PullTrigger(CityId cityId, Guid correlationId)
    {
        if (!Cannons.TryGetValue(cityId.Value, out var cannon) ||
            cannon.Status != CannonStatus.Firing ||
            cannon.Target is null)
            return;

        var target = new ShipId(Guid.Parse(cannon.Target));

        // Il grilletto e' stato premuto comunque: e' questo che conta per l'inceppamento.
        if ((cannon.Attempts + 1) % Armory.JamEveryShots == 0)
        {
            RaiseEvent(new EarthCannonJammed((EarthId)Id, cityId, target, correlationId));
            return;
        }

        var left = cannon.Rounds - 1;

        if (!Ships.TryGetValue(cannon.Target, out var ship))
        {
            RaiseEvent(new EarthShotWasted((EarthId)Id, cityId, target, left, correlationId));
        }
        else if (Armory.Misses(cannon.Attempts + 1))
        {
            // Il colpo e' partito e la munizione se n'e' andata: non c'e' niente da fare, il cannone
            // ricarica e riprova da solo. Serve solo a far durare di piu' la nave, e quindi a tenere
            // occupato piu' a lungo quel cannone.
            RaiseEvent(new EarthShotMissed((EarthId)Id, cityId, target, left, correlationId));
        }
        else
        {
            var hits = ship.Hits + 1;

            RaiseEvent(new EarthShotFired((EarthId)Id, cityId, target, hits, left, correlationId));

            if (hits >= Contracts.World.Ships.HitsToDestroy(ship.Class))
                RaiseEvent(new EarthShipDestroyed((EarthId)Id, new CityId(Guid.Parse(ship.CityId)), target,
                    correlationId));
        }

        if (left <= 0)
            RaiseEvent(new EarthCannonEmpty((EarthId)Id, cityId, target, correlationId));
    }

    /// <summary>Il tempo e' scaduto: la nave tocca terra.</summary>
    public void LandShip(ShipId shipId, Guid correlationId)
    {
        if (!Ships.TryGetValue(shipId.Value, out var ship))
            return;

        var cannon = Cannons[ship.CityId];
        var damage = Contracts.World.Ships.DamageOf(ship.Class);
        var integrityLeft = Math.Max(0, cannon.Integrity - damage);
        var cityId = new CityId(Guid.Parse(ship.CityId));

        RaiseEvent(new EarthShipLanded((EarthId)Id, cityId, shipId, damage, integrityLeft, correlationId));

        if (integrityLeft <= 0)
            RaiseEvent(new EarthCityFallen((EarthId)Id, cityId, shipId, correlationId));
    }

    public void Apply(EarthCommissioned @event)
    {
        Id = @event.AggregateId;
        Cannons = [];
        Ships = [];
    }

    public void Apply(EarthCityCommissioned @event) =>
        Cannons[@event.CityId.Value] = new Cannon
        {
            Name = @event.Name,
            Integrity = @event.Integrity,
            Rounds = @event.Rounds,
            Status = CannonStatus.Ready
        };

    public void Apply(EarthRecommissioned @event)
    {
        LastRecommissionedWave = @event.Wave;
        Ships = [];
        foreach (var cannon in Cannons.Values)
        {
            cannon.Integrity = @event.Integrity;
            cannon.Rounds = @event.Rounds;
            cannon.Status = CannonStatus.Ready;
            cannon.Target = null;
            cannon.JammedOn = null;
            cannon.Attempts = 0;
            cannon.Repairs = 0;
        }
    }

    public void Apply(EarthShipDetected @event) =>
        Ships[@event.ShipId.Value] = new Incoming(@event.CityId.Value, @event.ShipClass, 0);

    public void Apply(EarthNoCannonReady @event)
    {
    }

    public void Apply(EarthFireOpened @event)
    {
        var cannon = Cannons[@event.CityId.Value];
        cannon.Status = CannonStatus.Firing;
        cannon.Target = @event.ShipId.Value;
    }

    public void Apply(EarthFireCeased @event)
    {
        var cannon = Cannons[@event.CityId.Value];
        cannon.Target = null;
        cannon.Status = cannon.Rounds > 0 ? CannonStatus.Ready : CannonStatus.Empty;
    }

    public void Apply(EarthCannonJammed @event)
    {
        var cannon = Cannons[@event.CityId.Value];
        cannon.Attempts++;
        cannon.Target = null;
        cannon.JammedOn = @event.ShipId.Value;
        cannon.Status = CannonStatus.Jammed;
    }

    public void Apply(EarthCannonRepaired @event)
    {
        var cannon = Cannons[@event.CityId.Value];
        cannon.Repairs++;
        cannon.Rounds = @event.RoundsLeft;
        cannon.JammedOn = null;
        cannon.Status = CannonStatus.Ready;
    }

    /// <remarks>Resta inceppato e resta di chi ce l'ha: i colpi del tentativo pero' sono spesi.</remarks>
    public void Apply(EarthCannonStillJammed @event)
    {
        var cannon = Cannons[@event.CityId.Value];
        cannon.Repairs++;
        cannon.Rounds = @event.RoundsLeft;
    }

    /// <remarks>
    /// Il bersaglio <b>resta</b>. Un cannone a secco non e' un cannone libero: e' un cannone ancora
    /// assegnato a quella nave, che non le spara piu'. Finche' e' li', il battito la conta come
    /// coperta. Lo libera solo un cessate il fuoco.
    /// </remarks>
    public void Apply(EarthCannonEmpty @event)
    {
        var cannon = Cannons[@event.CityId.Value];
        cannon.Rounds = 0;
        cannon.JammedOn = null;
        cannon.Status = CannonStatus.Empty;
    }

    /// <remarks>
    /// Il colpo si applica sull'ingaggio ricostruito dall'evento e non su quello in memoria: comandi
    /// diversi hanno code diverse, quindi lo stream puo' contenere un colpo <b>dopo</b> l'atterraggio
    /// della stessa nave. Un <c>Apply</c> che desse per scontata la nave farebbe fallire la
    /// reidratazione per sempre, perche' lo stream non si riscrive.
    /// </remarks>
    public void Apply(EarthShotFired @event)
    {
        Shot(@event.CityId.Value, @event.RoundsLeft);

        if (Ships.TryGetValue(@event.ShipId.Value, out var ship))
            Ships[@event.ShipId.Value] = ship with { Hits = @event.Hits };
    }

    public void Apply(EarthShotMissed @event) => Shot(@event.CityId.Value, @event.RoundsLeft);

    public void Apply(EarthShotWasted @event) => Shot(@event.CityId.Value, @event.RoundsLeft);

    private void Shot(string cityId, int roundsLeft)
    {
        var cannon = Cannons[cityId];
        cannon.Attempts++;
        cannon.Rounds = roundsLeft;
    }

    public void Apply(EarthShipDestroyed @event) => Ships.Remove(@event.ShipId.Value);

    public void Apply(EarthShipLanded @event)
    {
        Ships.Remove(@event.ShipId.Value);
        Cannons[@event.CityId.Value].Integrity = @event.IntegrityLeft;
    }

    /// <remarks>La citta' e' perduta e il cannone con lei: non spara piu', qualunque cosa stesse facendo.</remarks>
    public void Apply(EarthCityFallen @event)
    {
        var cannon = Cannons[@event.CityId.Value];
        cannon.Integrity = 0;
        cannon.Target = null;
        cannon.JammedOn = null;
        cannon.Status = CannonStatus.Lost;
    }
}
