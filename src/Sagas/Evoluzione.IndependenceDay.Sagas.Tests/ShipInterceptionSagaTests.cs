using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Contracts.Commands.Sagas;
using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Contracts.Ids;
using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Sagas.ShipInterception;
using Evoluzione.IndependenceDay.Sagas.Tests.TestDoubles;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.CustomTypes;
using Muflone.Saga;
using Xunit;

namespace Evoluzione.IndependenceDay.Sagas.Tests;

/// <summary>
/// L'intercettazione, provata senza accendere niente: ne' bus, ne' Mongo, ne' event store.
/// </summary>
/// <remarks>
/// <b>Dieci test, dieci livelli.</b> Ogni test e' il gradino che serve a superare il livello con lo
/// stesso numero: con i primi tre verdi la campagna arriva in fondo al terzo livello senza perdere
/// una citta'. Vanno fatti diventare verdi <b>in ordine</b>: ognuno da' per scontato quello che c'e'
/// nei precedenti, e nessuno serve prima del suo livello.
/// <para>
/// E' una garanzia sul minimo: un gradino scritto bene puo' portare anche un po' piu' in la'. Quello
/// che non succede mai e' il contrario — saltarne uno ferma la campagna dove quel gradino serviva.
/// </para>
/// <para>
/// Al posto dell'infrastruttura ci sono tre sostituti — un bus che raccoglie invece di spedire, un
/// repository in memoria, nessuna ricerca per chiave di business. La saga e' quella vera, e gira in
/// un millisecondo.
/// </para>
/// </remarks>
public class ShipInterceptionSagaTests
{
    private static readonly Account Coordinator = new("saga", "Ship Interception");
    private static readonly EarthId Earth = new(Cities.DefenseId);

    private readonly Guid _correlationId = Guid.NewGuid();
    private readonly Guid _shipGuid = Guid.NewGuid();
    private readonly Guid _cityGuid = Cities.All[0].Id;
    private readonly Guid _gunGuid = Cities.All[1].Id;

    private readonly RecordingServiceBus _bus = new();
    private readonly InMemorySagaRepository _repository = new();

    private ShipInterceptionSaga Saga() =>
        new(_bus, _repository, new NoStateLocator(), new NullLoggerFactory());

    private ShipId Ship => new(_shipGuid);
    private CityId Gun => new(_gunGuid);
    private CityId Target => new(_cityGuid);

    private InterceptionState? State() => _repository.Peek<InterceptionState>(_correlationId);

    private bool StillOpen => _repository.Contains(_correlationId);

    /// <summary>La presa in carico: da qui in poi il processo esiste, e occupa una linea.</summary>
    private Task Start() =>
        Saga().StartedByAsync(new StartShipInterception(Ship, Target, _correlationId, Coordinator));

    /// <summary>Un cannone acceso: il punto di partenza di quasi tutto.</summary>
    private Task Firing() =>
        Saga().HandleAsync(new FireOpened(Earth, Gun, Ship, 100, _correlationId));

    /// <summary>L'ultimo ordine finito sul bus, che e' quello che i test guardano.</summary>
    private T Last<T>() => Assert.IsType<T>(_bus.Sent[^1]);

    // --- livello 1: aprire il fuoco ---------------------------------------------------------------

    /// <summary>Senza questo non spara nessuno, e la prima ondata rade al suolo cinque citta'.</summary>
    [Fact]
    public async Task Livello_1_la_prima_mossa_e_aprire_il_fuoco()
    {
        await Start();

        Assert.Equal(_shipGuid, Guid.Parse(Last<OpenFire>().ShipId.Value));
        Assert.NotNull(State());
    }

    // --- livello 2: restituire il cannone ---------------------------------------------------------

    /// <summary>
    /// Aprire il fuoco prende in prestito un cannone a tempo indeterminato. Al secondo livello le navi
    /// sono piu' dei cannoni, quindi vanno restituiti: uno tenuto e' uno che manca alla nave dopo.
    /// </summary>
    [Fact]
    public async Task Livello_2_quando_la_nave_cade_il_cannone_torna_indietro()
    {
        await Start();
        await Firing();

        Assert.Contains(_gunGuid, State()!.Firing);

        await Saga().HandleAsync(new ShipDestroyed(Earth, Ship, Target, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(Last<CeaseFire>().CityId.Value));
    }

    // --- livello 3: insistere ---------------------------------------------------------------------

    /// <summary>
    /// Un ordine su venticinque non arriva, e un ordine non arrivato non produce nessun evento.
    /// </summary>
    /// <remarks>
    /// Il battito con zero cannoni e' l'unico modo di accorgersene: chi aspetta un errore aspetta per
    /// sempre. E il battito con un cannone addosso non vuol dire niente, quindi non si fa niente —
    /// insistere li' vorrebbe dire togliere un cannone alla nave che arriva dopo.
    /// </remarks>
    [Fact]
    public async Task Livello_3_al_battito_si_insiste_solo_se_non_le_spara_nessuno()
    {
        await Start();
        await Firing();

        await Saga().HandleAsync(new ShipApproaching(Earth, Ship, Target, 6000, 1, _correlationId));

        Assert.Single(_bus.SentOf<OpenFire>());

        await Saga().HandleAsync(new ShipApproaching(Earth, Ship, Target, 5000, 0, _correlationId));

        Assert.Equal(2, _bus.SentOf<OpenFire>().Count());
    }

    // --- livello 4: richiudere --------------------------------------------------------------------

    /// <summary>
    /// Anche il cessate il fuoco puo' perdersi, e allora il cannone resta acceso su un relitto.
    /// </summary>
    /// <remarks>
    /// La nave e' risolta, quindi <see cref="ShipApproaching"/> non arriva piu': l'unico avviso e'
    /// questo, e continua finche' dura. Un cannone che brucia colpi su un rottame e' anche un cannone
    /// che non c'e' quando arriva la nave dopo.
    /// </remarks>
    [Fact]
    public async Task Livello_4_un_cannone_rimasto_acceso_si_richiude()
    {
        await Start();
        await Firing();
        await Saga().HandleAsync(new ShipDestroyed(Earth, Ship, Target, _correlationId));
        _bus.Sent.Clear();

        await Saga().HandleAsync(new CannonStillFiring(Earth, Gun, Ship, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(Last<CeaseFire>().CityId.Value));
    }

    // --- livello 5: riparare ----------------------------------------------------------------------

    /// <summary>
    /// Un cannone inceppato non spara e non si sblocca da solo: dimenticarlo e' perderlo per tutta la
    /// campagna. E non e' piu' un debito di fuoco — si e' fermato da solo — ma resta sul conto.
    /// </summary>
    [Fact]
    public async Task Livello_5_un_cannone_inceppato_si_fa_riparare()
    {
        await Start();
        await Firing();

        await Saga().HandleAsync(new CannonJammed(Earth, Gun, Ship, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(Last<RepairCannon>().CityId.Value));
        Assert.Empty(State()!.Firing);
    }

    // --- livello 6: insistere sulla riparazione ---------------------------------------------------

    /// <summary>
    /// La riparazione puo' non prendere, e l'ordine puo' perdersi: da fuori sono la stessa cosa.
    /// </summary>
    /// <remarks>
    /// I colpi del tentativo se ne sono andati lo stesso. Non c'e' niente da capire e niente da
    /// distinguere: si ripete finche' il cannone non torna.
    /// </remarks>
    [Fact]
    public async Task Livello_6_una_riparazione_che_non_prende_si_ripete()
    {
        await Start();
        await Firing();
        await Saga().HandleAsync(new CannonJammed(Earth, Gun, Ship, _correlationId));
        _bus.Sent.Clear();

        await Saga().HandleAsync(new CannonStillJammed(Earth, Gun, Ship, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(Last<RepairCannon>().CityId.Value));
    }

    // --- livello 7: chiudere ----------------------------------------------------------------------

    /// <summary>
    /// Un processo che non si chiude mai tiene la sua linea in sala operativa per sempre.
    /// </summary>
    /// <remarks>
    /// Non fa male subito, e per sei livelli non si vede. Poi le linee finiscono, e una nave viene
    /// avvistata senza che nessuno la prenda in carico: nessun ordine, nessun cannone, nessun errore.
    /// </remarks>
    [Fact]
    public async Task Livello_7_il_processo_si_chiude_quando_il_conto_e_saldato()
    {
        await Start();
        await Firing();

        await Saga().HandleAsync(new ShipDestroyed(Earth, Ship, Target, _correlationId));
        await Saga().HandleAsync(new FireCeased(Earth, Gun, Ship, 99, _correlationId));

        Assert.False(StillOpen, "nave abbattuta e cannone restituito: non c'e' piu' niente da seguire");
    }

    // --- livello 8: non chiudere troppo presto ----------------------------------------------------

    /// <summary>
    /// E' il punto in cui si sbaglia senza vedere un errore.
    /// </summary>
    /// <remarks>
    /// Chiudere quando la nave cade sembra corretto. Ma il cessate il fuoco puo' perdersi, e il
    /// richiamo del livello quattro arriva a un processo che non c'e' piu': quel cannone resta acceso
    /// per il resto della campagna, e nessuno lo verra' mai a sapere. Lo stesso vale per un cannone
    /// lasciato inceppato: e' rotto per colpa di questo processo, e nessun altro sa che esiste.
    /// </remarks>
    [Fact]
    public async Task Livello_8_il_processo_non_chiude_finche_il_conto_non_e_saldato()
    {
        await Start();
        await Firing();

        await Saga().HandleAsync(new ShipDestroyed(Earth, Ship, Target, _correlationId));

        Assert.True(StillOpen, "il cannone non e' ancora tornato indietro");

        await Saga().HandleAsync(new CannonJammed(Earth, Gun, Ship, _correlationId));

        Assert.True(StillOpen, "un cannone lasciato inceppato e' un conto aperto quanto uno acceso");

        await Saga().HandleAsync(new CannonRepaired(Earth, Gun, Ship, 96, _correlationId));

        Assert.False(StillOpen, "adesso si puo' chiudere");
    }

    // --- livello 9: non aspettare il battito ------------------------------------------------------

    /// <summary>
    /// Il battito e' una rete, non il meccanismo: aspettarlo quando un evento dice gia' tutto costa
    /// sette decimi di secondo su otto di finestra.
    /// </summary>
    /// <remarks>
    /// Riparare non riapre il fuoco: rimette il cannone disponibile, e fermo. Se la nave e' ancora in
    /// volo e non le spara nessuno, il cannone va rimesso in azione adesso.
    /// </remarks>
    [Fact]
    public async Task Livello_9_il_cannone_riparato_torna_subito_in_azione()
    {
        await Start();
        await Firing();
        await Saga().HandleAsync(new CannonJammed(Earth, Gun, Ship, _correlationId));
        _bus.Sent.Clear();

        await Saga().HandleAsync(new CannonRepaired(Earth, Gun, Ship, 96, _correlationId));

        Assert.Equal(_shipGuid, Guid.Parse(Last<OpenFire>().ShipId.Value));
    }

    // --- livello 10: il cannone a secco -----------------------------------------------------------

    /// <summary>
    /// Un cannone a secco resta <b>assegnato</b> alla sua nave finche' non lo si restituisce.
    /// </summary>
    /// <remarks>
    /// E' la trappola piu' silenziosa di tutte: finche' e' li', il battito conta quella nave come
    /// coperta — <c>CannonsFiring</c> e' uno — quindi l'insistenza del livello tre non scatta, e la
    /// nave arriva a terra con un cannone puntato addosso che non spara. Va restituito, e ne va
    /// chiesto un altro nello stesso momento.
    /// </remarks>
    [Fact]
    public async Task Livello_10_un_cannone_a_secco_si_restituisce_e_si_rimpiazza()
    {
        await Start();
        await Firing();
        _bus.Sent.Clear();

        await Saga().HandleAsync(new CannonEmpty(Earth, Gun, Ship, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(_bus.SentOf<CeaseFire>().Single().CityId.Value));
        Assert.Equal(_shipGuid, Guid.Parse(_bus.SentOf<OpenFire>().Single().ShipId.Value));
    }
}
