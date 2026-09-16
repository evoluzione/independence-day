using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Contracts.Commands.Sagas;
using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Contracts.Ids;
using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Sagas.ShipInterception;
using Muflone.CustomTypes;
using Xunit;

namespace Evoluzione.IndependenceDay.Sagas.Tests;

/// <summary>
/// Il processo, provato senza accendere niente: ne' bus, ne' Mongo, ne' event store.
/// </summary>
/// <remarks>
/// E' il motivo per cui la decisione sta in una classe a parte. Qui si prova quello che conta —
/// insistere quando manca un evento, riparare, e soprattutto <b>chiudere il conto</b> — con un test
/// che gira in un millisecondo.
/// </remarks>
public class InterceptionProcessTests
{
    private static readonly Account Coordinator = new("saga", "Ship Interception");
    private static readonly EarthId Earth = new(Cities.DefenseId);

    private readonly Guid _correlationId = Guid.NewGuid();
    private readonly Guid _shipGuid = Guid.NewGuid();
    private readonly Guid _cityGuid = Cities.All[0].Id;
    private readonly Guid _gunGuid = Cities.All[1].Id;

    private readonly InterceptionProcess _process = new();
    private readonly InterceptionState _state;

    public InterceptionProcessTests() =>
        _state = _process.Open(new StartShipInterception(new ShipId(_shipGuid), new CityId(_cityGuid),
            _correlationId, Coordinator));

    private ShipId Ship => new(_shipGuid);
    private CityId Gun => new(_gunGuid);

    private static T Single<T>(InterceptionReaction reaction) => Assert.IsType<T>(Assert.Single(reaction.Orders));

    /// <summary>Un cannone acceso: il punto di partenza di quasi tutto.</summary>
    private void Firing() =>
        _process.React(_state, new FireOpened(Earth, Gun, Ship, 100, _correlationId));

    [Fact]
    public void La_prima_mossa_e_aprire_il_fuoco()
    {
        var reaction = _process.FirstOrder(_state);

        Assert.Equal(_shipGuid, Guid.Parse(Single<OpenFire>(reaction).ShipId.Value));
    }

    /// <summary>
    /// Il battito con zero cannoni vuol dire che l'ordine si e' perso: si rifa'.
    /// </summary>
    /// <remarks>
    /// E' l'unico modo di accorgersi di un ordine perso, perche' un ordine perso non produce nessun
    /// evento. Chi aspetta un errore aspetta per sempre.
    /// </remarks>
    [Fact]
    public void Se_al_battito_non_le_spara_nessuno_il_fuoco_si_riapre()
    {
        var reaction = _process.React(_state,
            new ShipApproaching(Earth, Ship, new CityId(_cityGuid), 6000, 0, _correlationId));

        Single<OpenFire>(reaction);
    }

    [Fact]
    public void Se_al_battito_qualcuno_le_spara_non_si_fa_niente()
    {
        Firing();

        var reaction = _process.React(_state,
            new ShipApproaching(Earth, Ship, new CityId(_cityGuid), 6000, 1, _correlationId));

        Assert.Empty(reaction.Orders);
    }

    [Fact]
    public void Un_cannone_inceppato_si_fa_riparare()
    {
        Firing();

        var reaction = _process.React(_state, new CannonJammed(Earth, Gun, Ship, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(Single<RepairCannon>(reaction).CityId.Value));
        Assert.Empty(_state.Firing);
    }

    /// <summary>La compensazione: nave giu', cannoni indietro.</summary>
    [Fact]
    public void Quando_la_nave_cade_i_cannoni_aperti_vengono_chiusi()
    {
        Firing();

        var reaction = _process.React(_state, new ShipDestroyed(Earth, Ship, new CityId(_cityGuid), _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(Single<CeaseFire>(reaction).CityId.Value));
    }

    /// <summary>
    /// E non si chiude finche' la Terra non conferma di aver ripreso il cannone.
    /// </summary>
    /// <remarks>
    /// E' il punto in cui si sbaglia senza vedere un errore: chiudere qui sembra corretto, e lascia
    /// un cannone a sparare su relitti per il resto della campagna.
    /// </remarks>
    [Fact]
    public void Il_processo_non_chiude_finche_il_cannone_non_e_tornato()
    {
        Firing();

        var down = _process.React(_state, new ShipDestroyed(Earth, Ship, new CityId(_cityGuid), _correlationId));
        Assert.Equal(InterceptionOutcome.Continue, down.Outcome);

        var back = _process.React(_state, new FireCeased(Earth, Gun, Ship, 99, _correlationId));
        Assert.Equal(InterceptionOutcome.Won, back.Outcome);
    }

    /// <summary>Anche il cessate il fuoco puo' perdersi: la Terra lo ripete, e si insiste.</summary>
    [Fact]
    public void Un_cannone_ancora_acceso_si_richiude()
    {
        Firing();
        _process.React(_state, new ShipDestroyed(Earth, Ship, new CityId(_cityGuid), _correlationId));
        _process.React(_state, new FireCeased(Earth, Gun, Ship, 99, _correlationId));

        var reaction = _process.React(_state, new CannonStillFiring(Earth, Gun, Ship, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(Single<CeaseFire>(reaction).CityId.Value));
        Assert.Equal(InterceptionOutcome.Continue, reaction.Outcome);
    }

    /// <summary>
    /// Un'apertura che arriva a nave gia' caduta va chiusa subito.
    /// </summary>
    /// <remarks>
    /// L'ordine era per strada mentre la nave cadeva. Nessuno lo ha chiesto e nessuno lo aspetta:
    /// senza questo caso resta un cannone acceso che non compare in nessun conto.
    /// </remarks>
    [Fact]
    public void Un_fuoco_aperto_in_ritardo_si_chiude_subito()
    {
        _process.React(_state, new ShipDestroyed(Earth, Ship, new CityId(_cityGuid), _correlationId));

        var reaction = _process.React(_state, new FireOpened(Earth, Gun, Ship, 100, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(Single<CeaseFire>(reaction).CityId.Value));
    }

    [Fact]
    public void Una_nave_a_terra_chiude_male()
    {
        var reaction = _process.React(_state,
            new ShipLanded(Earth, Ship, new CityId(_cityGuid), 35, 65, _correlationId));

        Assert.Equal(InterceptionOutcome.Lost, reaction.Outcome);
    }
}
