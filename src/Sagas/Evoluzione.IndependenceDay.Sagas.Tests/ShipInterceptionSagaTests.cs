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
/// <b>Tre gradini, tre livelli.</b> I test sono raggruppati per livello, e il gruppo che porta il
/// numero N e' esattamente quello che serve a superare il livello N: con i gruppi fino a N verdi la
/// campagna arriva in fondo al livello N senza perdere una citta', e si ferma al successivo.
/// <para>
/// Non e' un margine: e' netto. Chi arriva al livello N+1 senza il suo gradino non perde una citta',
/// le perde <b>tutte e cinque</b>. I gradini sono tre perche' ognuno sia cosi'.
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

    // --- livello 2 (seguito): insistere quando c'e' silenzio -----------------------------------------------

    /// <summary>
    /// Un ordine su venticinque non arriva, e un ordine non arrivato non produce nessun evento.
    /// </summary>
    /// <remarks>
    /// Il battito e' l'unico modo di accorgersene: chi aspetta un errore aspetta per sempre. Con un
    /// cannone gia' addosso non si fa niente — insistere li' vorrebbe dire toglierlo alla nave che
    /// arriva dopo.
    /// </remarks>
    [Fact]
    public async Task Livello_2_al_battito_si_insiste_solo_se_non_le_spara_nessuno()
    {
        await Start();
        await Firing();

        await Saga().HandleAsync(new ShipApproaching(Earth, Ship, Target, 6000, 1, _correlationId));

        Assert.Single(_bus.SentOf<OpenFire>());

        await Saga().HandleAsync(new ShipApproaching(Earth, Ship, Target, 5000, 0, _correlationId));

        Assert.Equal(2, _bus.SentOf<OpenFire>().Count());
    }

    /// <summary>
    /// Anche il cessate il fuoco puo' perdersi, e allora il cannone resta acceso su un relitto.
    /// </summary>
    /// <remarks>
    /// La nave e' risolta, quindi <see cref="ShipApproaching"/> non arriva piu': l'unico avviso e'
    /// questo, e continua finche' dura. E' lo stesso gradino di sopra — c'e' silenzio dove dovrebbe
    /// esserci una risposta — visto dall'altra parte.
    /// </remarks>
    [Fact]
    public async Task Livello_2_un_cannone_rimasto_acceso_si_richiude()
    {
        await Start();
        await Firing();
        await Saga().HandleAsync(new ShipDestroyed(Earth, Ship, Target, _correlationId));
        _bus.Sent.Clear();

        await Saga().HandleAsync(new CannonStillFiring(Earth, Gun, Ship, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(Last<CeaseFire>().CityId.Value));
    }

    // --- livello 2 (seguito): riparare, e rimettere in azione ------------------------------------------------

    /// <summary>
    /// Un cannone inceppato non spara e non si sblocca da solo: dimenticarlo e' perderlo per tutta la
    /// campagna. Non e' piu' un debito di fuoco — si e' fermato da solo — ma resta sul conto.
    /// </summary>
    /// <remarks>
    /// E la riparazione puo' non prendere, o l'ordine puo' perdersi: da fuori sono la stessa cosa, e
    /// la risposta e' la stessa. Si ripete finche' il cannone non torna.
    /// </remarks>
    [Fact]
    public async Task Livello_2_un_cannone_inceppato_si_fa_riparare_e_si_insiste()
    {
        await Start();
        await Firing();

        await Saga().HandleAsync(new CannonJammed(Earth, Gun, Ship, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(Last<RepairCannon>().CityId.Value));
        Assert.Empty(State()!.Firing);

        _bus.Sent.Clear();
        await Saga().HandleAsync(new CannonStillJammed(Earth, Gun, Ship, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(Last<RepairCannon>().CityId.Value));
    }

    /// <summary>
    /// Riparare non riapre il fuoco: rimette il cannone disponibile, e fermo.
    /// </summary>
    /// <remarks>
    /// Rimetterlo in azione e' una mossa a parte, e va fatta adesso. Il battito ci arriverebbe, ma
    /// mezzo secondo dopo — e agli ultimi livelli mezzo secondo e' una nave a terra. Il battito e' una
    /// rete, non il meccanismo.
    /// </remarks>
    [Fact]
    public async Task Livello_2_il_cannone_riparato_torna_subito_in_azione()
    {
        await Start();
        await Firing();
        await Saga().HandleAsync(new CannonJammed(Earth, Gun, Ship, _correlationId));
        _bus.Sent.Clear();

        await Saga().HandleAsync(new CannonRepaired(Earth, Gun, Ship, 46, _correlationId));

        Assert.Equal(_shipGuid, Guid.Parse(Last<OpenFire>().ShipId.Value));
    }

    // --- livello 3: chiudere il conto --------------------------------------------------------------

    /// <summary>
    /// Un processo che non si chiude mai tiene la sua linea in sala operativa per sempre.
    /// </summary>
    /// <remarks>
    /// Non fa male subito, e per due livelli non si vede. Poi le linee finiscono, e una nave viene
    /// avvistata senza che nessuno la prenda in carico: nessun ordine, nessun cannone, nessun errore.
    /// </remarks>
    [Fact]
    public async Task Livello_3_il_processo_si_chiude_quando_il_conto_e_saldato()
    {
        await Start();
        await Firing();

        await Saga().HandleAsync(new ShipDestroyed(Earth, Ship, Target, _correlationId));
        await Saga().HandleAsync(new FireCeased(Earth, Gun, Ship, 49, _correlationId));

        Assert.False(StillOpen, "nave abbattuta e cannone restituito: non c'e' piu' niente da seguire");
    }

    /// <summary>
    /// E' il punto in cui si sbaglia senza vedere un errore.
    /// </summary>
    /// <remarks>
    /// Chiudere quando la nave cade sembra corretto. Ma il cessate il fuoco puo' perdersi, e il
    /// richiamo del secondo livello arriva a un processo che non c'e' piu': quel cannone resta acceso
    /// per il resto della campagna, e nessuno lo verra' mai a sapere. Lo stesso vale per un cannone
    /// lasciato inceppato: e' rotto per colpa di questo processo, e nessun altro sa che esiste.
    /// </remarks>
    [Fact]
    public async Task Livello_3_il_processo_non_chiude_finche_il_conto_non_e_saldato()
    {
        await Start();
        await Firing();

        await Saga().HandleAsync(new ShipDestroyed(Earth, Ship, Target, _correlationId));

        Assert.True(StillOpen, "il cannone non e' ancora tornato indietro");

        await Saga().HandleAsync(new CannonJammed(Earth, Gun, Ship, _correlationId));

        Assert.True(StillOpen, "un cannone lasciato inceppato e' un conto aperto quanto uno acceso");

        await Saga().HandleAsync(new CannonRepaired(Earth, Gun, Ship, 46, _correlationId));

        Assert.False(StillOpen, "adesso si puo' chiudere");
    }

    /// <summary>
    /// Un cannone a secco resta <b>assegnato</b> alla sua nave finche' non lo si restituisce.
    /// </summary>
    /// <remarks>
    /// E' la trappola piu' silenziosa di tutte, ed e' l'ultima faccia dello stesso conto: finche' e'
    /// li', il battito conta quella nave come coperta — <c>CannonsFiring</c> e' uno — quindi
    /// l'insistenza del secondo livello non scatta, e la nave arriva a terra con un cannone puntato
    /// addosso che non spara. Va restituito, e ne va chiesto un altro nello stesso momento.
    /// </remarks>
    [Fact]
    public async Task Livello_3_un_cannone_a_secco_si_restituisce_e_si_rimpiazza()
    {
        await Start();
        await Firing();
        _bus.Sent.Clear();

        await Saga().HandleAsync(new CannonEmpty(Earth, Gun, Ship, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(_bus.SentOf<CeaseFire>().Single().CityId.Value));
        Assert.Equal(_shipGuid, Guid.Parse(_bus.SentOf<OpenFire>().Single().ShipId.Value));
    }
}
