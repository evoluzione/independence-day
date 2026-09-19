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

    private Task Start() =>
    Saga().StartedByAsync(new StartShipInterception(Ship, Target, _correlationId, Coordinator));

    private Task Firing() =>
    Saga().HandleAsync(new FireOpened(Earth, Gun, Ship, 100, _correlationId));

    private T Last<T>() => Assert.IsType<T>(_bus.Sent[^1]);

    [Fact]
    public async Task Una_nave_avvistata_apre_il_fuoco_e_si_insiste_se_resta_scoperta()
    {
        await Start();

        Assert.Equal(_shipGuid, Guid.Parse(Last<OpenFire>().ShipId.Value));

        await Saga().HandleAsync(new ShipApproaching(Earth, Ship, Target, 6000, 1, _correlationId));
        Assert.Single(_bus.SentOf<OpenFire>());

        await Saga().HandleAsync(new ShipApproaching(Earth, Ship, Target, 5000, 0, _correlationId));
        Assert.Equal(2, _bus.SentOf<OpenFire>().Count());
    }

    [Fact]
    public async Task La_nave_abbattuta_restituisce_il_cannone()
    {
        await Start();
        await Firing();

        await Saga().HandleAsync(new ShipDestroyed(Earth, Ship, Target, _correlationId));

        Assert.Equal(_shipGuid, Guid.Parse(Last<CeaseFire>().ShipId.Value));
    }

    [Fact]
    public async Task Il_cannone_inceppato_si_fa_riparare()
    {
        await Start();
        await Firing();

        await Saga().HandleAsync(new CannonJammed(Earth, Gun, Ship, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(Last<RepairCannon>().CityId.Value));
    }

    [Fact]
    public async Task Il_cannone_riparato_torna_subito_in_azione()
    {
        await Start();
        await Firing();
        await Saga().HandleAsync(new CannonJammed(Earth, Gun, Ship, _correlationId));
        _bus.Sent.Clear();

        await Saga().HandleAsync(new CannonRepaired(Earth, Gun, Ship, 46, _correlationId));

        Assert.Equal(_shipGuid, Guid.Parse(Last<OpenFire>().ShipId.Value));
    }

    [Fact]
    public async Task Il_cannone_a_secco_chiede_i_rifornimenti_e_si_rimette_in_azione()
    {
        await Start();
        await Firing();
        _bus.Sent.Clear();

        await Saga().HandleAsync(new CannonEmpty(Earth, Gun, Ship, _correlationId));

        Assert.Equal(_gunGuid, Guid.Parse(Last<RequestResupply>().CityId.Value));

        await Saga().HandleAsync(new CannonResupplied(Earth, Gun, Ship, 20, _correlationId));

        Assert.Equal(_shipGuid, Guid.Parse(Last<OpenFire>().ShipId.Value));
    }
}
