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
/// Al posto dell'infrastruttura ci sono tre sostituti — un bus che raccoglie invece di spedire, un
/// repository in memoria, nessuna ricerca per chiave di business. La saga e' quella vera, e gira in
/// un millisecondo.
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

    /// <summary>La presa in carico: da qui in poi il processo esiste.</summary>
    private Task Start() =>
        Saga().StartedByAsync(new StartShipInterception(Ship, Target, _correlationId, Coordinator));

    /// <summary>Un cannone acceso: il punto di partenza di quasi tutto.</summary>
    private Task Firing() =>
        Saga().HandleAsync(new FireOpened(Earth, Gun, Ship, 100, _correlationId));

    /// <summary>L'ultimo ordine finito sul bus, che e' quello che i test guardano.</summary>
    private T Last<T>() => Assert.IsType<T>(_bus.Sent[^1]);

    [Fact]
    public async Task La_prima_mossa_e_aprire_il_fuoco()
    {
        await Start();

        Assert.Equal(_shipGuid, Guid.Parse(Last<OpenFire>().ShipId.Value));
        Assert.NotNull(State());
    }

    /// <summary>
    /// Il battito con zero cannoni vuol dire che l'ordine si e' perso: si rifa'.
    /// </summary>
    /// <remarks>
    /// E' l'unico modo di accorgersi di un ordine perso, perche' un ordine perso non produce nessun
    /// evento. Chi aspetta un errore aspetta per sempre.
    /// </remarks>
    [Fact]
    public async Task Se_al_battito_non_le_spara_nessuno_il_fuoco_si_riapre()
    {
        await Start();

        await Saga().HandleAsync(new ShipApproaching(Earth, Ship, Target, 6000, 0, _correlationId));

        Assert.Equal(2, _bus.SentOf<OpenFire>().Count());
    }

    [Fact]
    public async Task Se_al_battito_qualcuno_le_spara_non_si_fa_niente()
    {
        await Start();
        await Firing();

        await Saga().HandleAsync(new ShipApproaching(Earth, Ship, Target, 6000, 1, _correlationId));

        Assert.Single(_bus.SentOf<OpenFire>());
    }

    [Fact]
    public async Task Un_cannone_inceppato_si_fa_riparare()
    {
        await Start();
        await Firing();

        await Saga().HandleAsync(new CannonJammed(Earth, Gun, Ship, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(Last<RepairCannon>().CityId.Value));
        Assert.Empty(State()!.Firing);
    }

    /// <summary>La compensazione: nave giu', cannoni indietro.</summary>
    [Fact]
    public async Task Quando_la_nave_cade_i_cannoni_aperti_vengono_chiusi()
    {
        await Start();
        await Firing();

        await Saga().HandleAsync(new ShipDestroyed(Earth, Ship, Target, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(Last<CeaseFire>().CityId.Value));
    }

    /// <summary>
    /// E non si chiude finche' la Terra non conferma di aver ripreso il cannone.
    /// </summary>
    /// <remarks>
    /// E' il punto in cui si sbaglia senza vedere un errore: chiudere qui sembra corretto, e lascia
    /// un cannone a sparare su un relitto per il resto della campagna.
    /// </remarks>
    [Fact]
    public async Task Il_processo_non_chiude_finche_il_cannone_non_e_tornato()
    {
        await Start();
        await Firing();

        await Saga().HandleAsync(new ShipDestroyed(Earth, Ship, Target, _correlationId));
        Assert.True(_repository.Contains(_correlationId), "il conto con la Terra e' ancora aperto");

        await Saga().HandleAsync(new FireCeased(Earth, Gun, Ship, 99, _correlationId));
        Assert.False(_repository.Contains(_correlationId), "adesso si puo' chiudere");
    }

    /// <summary>
    /// Anche il cessate il fuoco puo' perdersi: la Terra lo ripete, e si insiste.
    /// </summary>
    /// <remarks>
    /// Il cessate il fuoco non arriva mai alla Terra, quindi nessun <c>FireCeased</c> torna indietro:
    /// il conto resta aperto, e il processo con lui. E' proprio perche' non ha chiuso che puo'
    /// ricevere il richiamo e rimediare.
    /// </remarks>
    [Fact]
    public async Task Un_cannone_ancora_acceso_si_richiude()
    {
        await Start();
        await Firing();
        await Saga().HandleAsync(new ShipDestroyed(Earth, Ship, Target, _correlationId));
        _bus.Sent.Clear();

        await Saga().HandleAsync(new CannonStillFiring(Earth, Gun, Ship, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(Last<CeaseFire>().CityId.Value));
        Assert.True(_repository.Contains(_correlationId), "con un cannone acceso non si chiude");
    }

    /// <summary>
    /// Un'apertura che arriva a nave gia' caduta va chiusa subito.
    /// </summary>
    /// <remarks>
    /// Il primo cannone ha abbattuto la nave mentre l'ordine per un secondo era ancora per strada.
    /// Nessuno lo ha chiesto e nessuno lo aspetta: senza chiuderlo subito resta un cannone acceso che
    /// non compare in nessun conto.
    /// </remarks>
    [Fact]
    public async Task Un_fuoco_aperto_in_ritardo_si_chiude_subito()
    {
        var secondo = new CityId(Cities.All[2].Id);

        await Start();
        await Firing();
        await Saga().HandleAsync(new ShipDestroyed(Earth, Ship, Target, _correlationId));
        _bus.Sent.Clear();

        await Saga().HandleAsync(new FireOpened(Earth, secondo, Ship, 100, _correlationId));

        Assert.Equal(secondo.Value, Last<CeaseFire>().CityId.Value);
    }

    [Fact]
    public async Task Una_nave_a_terra_chiude_male()
    {
        await Start();

        await Saga().HandleAsync(new ShipLanded(Earth, Ship, Target, 100, 0, _correlationId));

        Assert.Equal(SagaStatus.Failed, State()?.Status);
    }
}
