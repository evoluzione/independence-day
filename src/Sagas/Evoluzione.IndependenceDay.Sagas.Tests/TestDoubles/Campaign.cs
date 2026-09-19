using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Contracts.Commands.Sagas;
using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.Entities;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Evoluzione.IndependenceDay.Sagas.ShipInterception;
using Evoluzione.IndependenceDay.Space.Domain.Services;
using Muflone;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.CustomTypes;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using C = Evoluzione.IndependenceDay.Contracts.Events;
using EarthId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.EarthId;
using EarthCityId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.CityId;
using EarthShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Sagas.Tests.TestDoubles;

[Flags]
public enum Skills
{
    None = 0,

    Fire = 1,

    Cease = 2,

    Repair = 4,

    Resume = 8,

    Resupply = 16,

    All = Fire | Cease | Repair | Resume | Resupply
}

public sealed record CampaignResult(
    int CitiesStanding,
    int ShipsDestroyed,
    int ShipsLanded,
    int RoundsSpent,
    int RoundsMissed,
    int RoundsWasted,
    int RoundsLeft,
    int Jams,
    int OpenCannons,
    int Empties,
    int Resupplies)
{
    public bool Won => CitiesStanding > 0;

    public override string ToString() =>
        $"{CitiesStanding} citta' in piedi, {ShipsDestroyed} abbattute, {ShipsLanded} atterrate, " +
        $"{RoundsSpent} colpi ({RoundsMissed} mancati, {RoundsWasted} su relitti), {RoundsLeft} rimasti, " +
        $"{Jams} inceppamenti, {OpenCannons} cannoni lasciati accesi, {Empties} cannoni a secco, " +
        $"{Resupplies} consegne";
}

public sealed class Campaign
{
    private const int TickMs = 100;
    private const int HeartbeatMs = 500;

    private const int LaunchIntervalMs = 1000;

    private static readonly Account Who = new("test", "Campaign");
    private static readonly EarthId Earth = new(Cities.DefenseId);
    private static readonly Contracts.Ids.EarthId ContractsEarth = new(Cities.DefenseId);

    private readonly RecordingServiceBus _bus = new();
    private readonly InMemorySagaRepository _sagas = new();
    private readonly HashSet<Guid> _started = [];
    private readonly Dictionary<Guid, DateTime> _detectedAt = [];
    private readonly Dictionary<Guid, DateTime> _lastShotAt = [];

    private readonly Dictionary<Guid, (DateTime Due, Guid ShipId)> _resupplyDue = [];

    private EarthDefense _earth = null!;
    private DateTime _now = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private int _spent;
    private int _missed;
    private int _wasted;
    private int _jams;
    private int _destroyed;
    private int _landed;
    private int _empties;
    private int _resupplies;

    public Skills Skills { get; init; } = Skills.All;

    private readonly WaveDifficulty _difficulty = new();

    public CampaignResult Play()
    {
        _earth = EarthDefense.Commission(Earth, Armory.RoundsPerCity, Ships.FullIntegrity, Guid.NewGuid());
        Drain();

        Wave(_difficulty.Plan());
        Settle();

        return new CampaignResult(Standing().Count, _destroyed, _landed, _spent, _missed, _wasted,
            _earth.Cannons.Values.Where(c => c.Status != CannonStatus.Lost).Sum(c => c.Rounds),
            _jams, _earth.Cannons.Values.Count(c => c.Status == CannonStatus.Firing), _empties, _resupplies);
    }

    private void Wave(WavePlan plan)
    {
        var launched = 0;
        var lastLaunch = DateTime.MinValue;
        var lastBeat = _now;

        for (var tick = 0; tick < 4000; tick++)
        {
            var targets = Standing();
            if (targets.Count == 0)
                return;

            if (launched < plan.Count && _now - lastLaunch >= TimeSpan.FromMilliseconds(LaunchIntervalMs))
            {
                Launch(plan.Ships[launched], targets[launched % targets.Count]);
                lastLaunch = _now;
                launched++;
            }

            Triggers();
            Landings();
            Resupplies();

            if (_now - lastBeat >= TimeSpan.FromMilliseconds(HeartbeatMs))
            {
                Beats();
                lastBeat = _now;
            }

            if (launched >= plan.Count && _earth.Ships.Count == 0)
                return;

            _now = _now.AddMilliseconds(TickMs);
        }

        throw new InvalidOperationException("L'ondata non si e' mai chiusa: c'e' un giro che non avanza.");
    }

    private void Settle()
    {
        for (var tick = 0; tick < 60 &&
             _earth.Cannons.Values.Any(c => c.Status is CannonStatus.Firing or CannonStatus.Resupplying); tick++)
        {
            Triggers();
            Resupplies();
            Beats();
            _now = _now.AddMilliseconds(HeartbeatMs);
        }
    }

    private void Launch(ShipClass shipClass, Guid cityId)
    {
        var shipId = Guid.NewGuid();
        _detectedAt[shipId] = _now;

        _earth.DetectShip(new EarthCityId(cityId), new EarthShipId(shipId), shipClass, Guid.NewGuid());
        Drain();

        _started.Add(shipId);
        Saga().StartedByAsync(new StartShipInterception(new Contracts.Ids.ShipId(shipId),
            new Contracts.Ids.CityId(cityId), shipId, Who)).GetAwaiter().GetResult();
        Execute();
    }

    private void Triggers()
    {
        foreach (var (cityId, cannon) in _earth.Cannons.ToList())
        {
            if (cannon.Status != CannonStatus.Firing)
                continue;

            if (_lastShotAt.TryGetValue(Guid.Parse(cityId), out var last) &&
                _now - last < TimeSpan.FromMilliseconds(Armory.ReloadMs))
                continue;

            _lastShotAt[Guid.Parse(cityId)] = _now;
            _earth.PullTrigger(new EarthCityId(Guid.Parse(cityId)), Guid.NewGuid());
            Drain();
        }
    }

    private void Landings()
    {
        var deadline = _now.AddSeconds(-Invasion.ApproachSeconds);

        foreach (var shipId in _earth.Ships.Keys.ToList())
        {
            var id = Guid.Parse(shipId);
            if (_detectedAt[id] > deadline)
                continue;

            _earth.LandShip(new EarthShipId(id), Guid.NewGuid());
            Drain();
        }
    }

    private void Resupplies()
    {
        foreach (var (cityId, due) in _resupplyDue.ToList())
        {
            if (_now < due.Due)
                continue;

            _resupplyDue.Remove(cityId);
            _earth.DeliverSupplies(new EarthCityId(cityId), new EarthShipId(due.ShipId), Guid.NewGuid());
            Drain();
        }
    }

    private void Beats()
    {
        var approach = Invasion.ApproachSeconds * 1000;

        foreach (var (shipId, ship) in _earth.Ships.ToList())
        {
            var id = Guid.Parse(shipId);
            var firing = _earth.Cannons.Values.Count(c => c.Target == shipId);

            Deliver(id, new C.ShipApproaching(ContractsEarth, new Contracts.Ids.ShipId(id),
                new Contracts.Ids.CityId(Guid.Parse(ship.CityId)),
                Math.Max(0, approach - (int)(_now - _detectedAt[id]).TotalMilliseconds), firing, id));
        }
    }

    private void Drain()
    {
        var events = ((IAggregate)_earth).GetUncommittedEvents().OfType<DomainEvent>().ToList();
        ((IAggregate)_earth).ClearUncommittedEvents();

        foreach (var @event in events)
        {
            Count(@event);
            Schedule(@event);

            var translated = Translate(@event);
            if (translated is not null)
                Deliver(ShipOf(@event), translated);
        }
    }

    private void Count(DomainEvent @event)
    {
        switch (@event)
        {
            case EarthShotFired: _spent++; break;
            case EarthShotMissed: _spent++; _missed++; break;
            case EarthShotWasted: _spent++; _wasted++; break;
            case EarthCannonJammed: _jams++; break;
            case EarthCannonEmpty: _empties++; break;
            case EarthCannonResupplied: _resupplies++; break;
            case EarthShipDestroyed: _destroyed++; break;
            case EarthShipLanded: _landed++; break;
        }
    }

    private void Schedule(DomainEvent @event)
    {
        if (@event is EarthResupplyDispatched dispatched)
            _resupplyDue[Guid.Parse(dispatched.CityId.Value)] =
                (_now.AddSeconds(Armory.ResupplySeconds), Guid.Parse(dispatched.ShipId.Value));
    }

    private static Guid ShipOf(DomainEvent @event) => @event switch
    {
        EarthFireOpened e => Guid.Parse(e.ShipId.Value),
        EarthFireCeased e => Guid.Parse(e.ShipId.Value),
        EarthCannonJammed e => Guid.Parse(e.ShipId.Value),
        EarthCannonRepaired e => Guid.Parse(e.ShipId.Value),
        EarthCannonEmpty e => Guid.Parse(e.ShipId.Value),
        EarthCannonResupplied e => Guid.Parse(e.ShipId.Value),
        EarthShipDestroyed e => Guid.Parse(e.ShipId.Value),
        EarthShipLanded e => Guid.Parse(e.ShipId.Value),
        _ => Guid.Empty
    };

    private static Event? Translate(DomainEvent @event)
    {
        var city = (Func<EarthCityId, Contracts.Ids.CityId>)(id => new Contracts.Ids.CityId(Guid.Parse(id.Value)));
        var ship = (Func<EarthShipId, Contracts.Ids.ShipId>)(id => new Contracts.Ids.ShipId(Guid.Parse(id.Value)));

        return @event switch
        {
            EarthFireOpened e => new C.FireOpened(ContractsEarth, city(e.CityId), ship(e.ShipId), e.RoundsLeft,
                Guid.Parse(e.ShipId.Value)),
            EarthFireCeased e => new C.FireCeased(ContractsEarth, city(e.CityId), ship(e.ShipId), e.RoundsLeft,
                Guid.Parse(e.ShipId.Value)),
            EarthCannonJammed e => new C.CannonJammed(ContractsEarth, city(e.CityId), ship(e.ShipId),
                Guid.Parse(e.ShipId.Value)),
            EarthCannonRepaired e => new C.CannonRepaired(ContractsEarth, city(e.CityId), ship(e.ShipId),
                e.RoundsLeft, Guid.Parse(e.ShipId.Value)),
            EarthCannonEmpty e => new C.CannonEmpty(ContractsEarth, city(e.CityId), ship(e.ShipId),
                Guid.Parse(e.ShipId.Value)),
            EarthCannonResupplied e => new C.CannonResupplied(ContractsEarth, city(e.CityId), ship(e.ShipId),
                e.Rounds, Guid.Parse(e.ShipId.Value)),
            EarthShipDestroyed e => new C.ShipDestroyed(ContractsEarth, ship(e.ShipId), city(e.CityId),
                Guid.Parse(e.ShipId.Value)),
            EarthShipLanded e => new C.ShipLanded(ContractsEarth, ship(e.ShipId), city(e.CityId), e.Damage,
                e.IntegrityLeft, Guid.Parse(e.ShipId.Value)),
            _ => null
        };
    }

    private void Deliver(Guid shipId, Event @event)
    {
        if (!_started.Contains(shipId))
            return;

        if (@event is C.CannonRepaired && !Skills.HasFlag(Skills.Resume))
            return;

        switch (@event)
        {
            case C.ShipApproaching e: Saga().HandleAsync(e).GetAwaiter().GetResult(); break;
            case C.FireOpened e: Saga().HandleAsync(e).GetAwaiter().GetResult(); break;
            case C.FireCeased e: Saga().HandleAsync(e).GetAwaiter().GetResult(); break;
            case C.CannonJammed e: Saga().HandleAsync(e).GetAwaiter().GetResult(); break;
            case C.CannonRepaired e: Saga().HandleAsync(e).GetAwaiter().GetResult(); break;
            case C.CannonEmpty e: Saga().HandleAsync(e).GetAwaiter().GetResult(); break;
            case C.CannonResupplied e: Saga().HandleAsync(e).GetAwaiter().GetResult(); break;
            case C.ShipDestroyed e: Saga().HandleAsync(e).GetAwaiter().GetResult(); break;
            case C.ShipLanded e: Saga().HandleAsync(e).GetAwaiter().GetResult(); break;
        }

        Execute();
    }

    private ShipInterceptionSaga Saga() =>
        new(_bus, _sagas, new NoStateLocator(), new NullLoggerFactory());

    private void Execute()
    {
        var orders = _bus.Sent.OfType<Command>().ToList();
        _bus.Sent.Clear();

        foreach (var order in orders)
            Apply(order);
    }

    private void Apply(Command order)
    {
        switch (order)
        {
            case OpenFire fire:
                _earth.OpenFire(new EarthShipId(Guid.Parse(fire.ShipId.Value)), Guid.NewGuid());
                break;
            case CeaseFire cease when Skills.HasFlag(Skills.Cease):
                _earth.CeaseFire(new EarthShipId(Guid.Parse(cease.ShipId.Value)), Guid.NewGuid());
                break;
            case RepairCannon repair when Skills.HasFlag(Skills.Repair):
                _earth.RepairCannon(new EarthCityId(Guid.Parse(repair.CityId.Value)),
                    new EarthShipId(Guid.Parse(repair.ShipId.Value)), Guid.NewGuid());
                break;
            case RequestResupply resupply when Skills.HasFlag(Skills.Resupply):
                _earth.RequestResupply(new EarthCityId(Guid.Parse(resupply.CityId.Value)),
                    new EarthShipId(Guid.Parse(resupply.ShipId.Value)), Guid.NewGuid());
                break;
            default:
                return;
        }

        Drain();
    }

    private List<Guid> Standing() =>
        _earth.Cannons.Where(c => !c.Value.Fallen).Select(c => Guid.Parse(c.Key)).ToList();
}
