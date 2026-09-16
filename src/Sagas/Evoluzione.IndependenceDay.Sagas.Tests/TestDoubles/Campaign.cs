using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Contracts.Commands.Sagas;
using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.Entities;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Evoluzione.IndependenceDay.Sagas.ShipInterception;
using Evoluzione.IndependenceDay.Space.Domain.Services;
using Muflone;
using Muflone.CustomTypes;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using C = Evoluzione.IndependenceDay.Contracts.Events;
using EarthId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.EarthId;
using EarthCityId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.CityId;
using EarthShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Sagas.Tests.TestDoubles;

/// <summary>Com'e' finita una campagna simulata.</summary>
public sealed record CampaignResult(
    int LevelReached,
    int CitiesStanding,
    int ShipsDestroyed,
    int ShipsLanded,
    int RoundsSpent,
    int RoundsMissed,
    int RoundsWasted,
    int RoundsLeft,
    int Jams,
    int OrdersLost,
    int OpenCannons)
{
    public bool Won => LevelReached >= Invasion.LastLevel && CitiesStanding > 0;

    public override string ToString() =>
        $"livello {LevelReached}, {CitiesStanding} citta' in piedi, {ShipsDestroyed} abbattute, " +
        $"{ShipsLanded} atterrate, {RoundsSpent} colpi ({RoundsMissed} mancati, {RoundsWasted} su relitti), " +
        $"{RoundsLeft} rimasti, " +
        $"{Jams} inceppamenti, {OrdersLost} ordini persi, {OpenCannons} cannoni lasciati accesi";
}

/// <summary>
/// La campagna intera, giocata in memoria: niente bus, niente Mongo, niente event store.
/// </summary>
/// <remarks>
/// Usa l'aggregato vero della Terra e il processo vero — non finte copie — e al posto
/// dell'infrastruttura mette un orologio che avanza a passi. E' lo strumento con cui si tara il
/// bilanciamento: cambiare un numero in <see cref="Armory"/> o in <see cref="WaveDifficulty"/> e
/// rilanciare i test dice subito se il gioco e' diventato impossibile o banale.
/// <para>
/// Una semplificazione c'e', ed e' voluta: i comandi arrivano all'aggregato nello stesso passo in cui
/// vengono impartiti, mentre in produzione ci sono una coda e un event store in mezzo. La simulazione
/// e' quindi un filo piu' generosa del gioco vero — va bene, perche' serve a dire che una strategia
/// <b>non</b> basta, e quella conclusione regge a maggior ragione.
/// </para>
/// </remarks>
public sealed class Campaign
{
    private const int TickMs = 100;
    private const int HeartbeatMs = 500;
    private const int LaunchIntervalMs = 1300;

    private static readonly Account Who = new("test", "Campaign");
    private static readonly EarthId Earth = new(Cities.DefenseId);
    private static readonly Contracts.Ids.EarthId ContractsEarth = new(Cities.DefenseId);

    private readonly WaveDifficulty _difficulty = new();
    private readonly InterceptionProcess _process = new();
    private readonly Dictionary<Guid, InterceptionState> _states = [];
    private readonly Dictionary<Guid, DateTime> _detectedAt = [];
    private readonly Dictionary<Guid, DateTime> _lastShotAt = [];

    private EarthDefense _earth = null!;
    private DateTime _now = new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    private int _orders;
    private int _spent;
    private int _missed;
    private int _wasted;
    private int _jams;
    private int _lost;
    private int _destroyed;
    private int _landed;

    /// <summary>Se falso, il processo non restituisce mai un cannone: e' la prova del contrario.</summary>
    public bool Compensates { get; init; } = true;

    public CampaignResult Play()
    {
        _earth = EarthDefense.Commission(Earth, Armory.RoundsPerCity, Ships.FullIntegrity, Guid.NewGuid());
        Drain();

        var level = 0;
        for (var l = 1; l <= Invasion.LastLevel; l++)
        {
            level = l;
            Wave(_difficulty.For(l));
            Settle();

            if (Standing().Count == 0)
                break;
        }

        return new CampaignResult(level, Standing().Count, _destroyed, _landed, _spent, _missed, _wasted,
            _earth.Cannons.Values.Where(c => c.Status != CannonStatus.Lost).Sum(c => c.Rounds),
            _jams, _lost,
            _earth.Cannons.Values.Count(c => c.Status == CannonStatus.Firing));
    }

    private void Wave(WavePlan plan)
    {
        var launched = 0;
        var lastLaunch = DateTime.MinValue;
        var lastBeat = _now;

        // Un'ondata dura finche' ci sono navi da lanciare o navi in volo. Il tetto di giri e' una
        // rete: se qualcosa si incastra il test fallisce invece di girare per sempre.
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

    /// <summary>
    /// Fra un'ondata e l'altra il battito continua: e' li' che si rimedia a un cessate il fuoco
    /// perso all'ultimo istante.
    /// </summary>
    /// <remarks>
    /// Nel gioco vero non esiste un "fra un'ondata e l'altra" per la Terra: i giri di fondo non si
    /// fermano mai. La simulazione lavora a ondate, quindi deve ricrearlo — altrimenti misurerebbe
    /// un cannone acceso che nella realta' si sarebbe chiuso da solo un secondo dopo.
    /// </remarks>
    private void Settle()
    {
        for (var tick = 0; tick < 60 && _earth.Cannons.Values.Any(c => c.Status == CannonStatus.Firing); tick++)
        {
            Triggers();
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

        // L'avvistamento accende il processo, come fa l'handler di integrazione nel gioco vero.
        var state = _process.Open(new StartShipInterception(new Contracts.Ids.ShipId(shipId),
            new Contracts.Ids.CityId(cityId), shipId, Who));
        _states[shipId] = state;

        Execute(_process.FirstOrder(state));
    }

    /// <summary>La centrale di tiro: un colpo per ogni cannone che ha finito di ricaricare.</summary>
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

    /// <summary>Il battito: navi ancora vive, e cannoni rimasti puntati su relitti.</summary>
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

        foreach (var (cityId, cannon) in _earth.Cannons.ToList())
        {
            if (cannon.Target is null || _earth.Ships.ContainsKey(cannon.Target))
                continue;

            Deliver(Guid.Parse(cannon.Target), new C.CannonStillFiring(ContractsEarth,
                new Contracts.Ids.CityId(Guid.Parse(cityId)),
                new Contracts.Ids.ShipId(Guid.Parse(cannon.Target)), Guid.Parse(cannon.Target)));
        }
    }

    /// <summary>Porta fuori dall'aggregato quello che e' appena successo, e lo consegna ai processi.</summary>
    private void Drain()
    {
        var events = ((IAggregate)_earth).GetUncommittedEvents().OfType<DomainEvent>().ToList();
        ((IAggregate)_earth).ClearUncommittedEvents();

        foreach (var @event in events)
        {
            Count(@event);

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
            case EarthShipDestroyed: _destroyed++; break;
            case EarthShipLanded: _landed++; break;
        }
    }

    private static Guid ShipOf(DomainEvent @event) => @event switch
    {
        EarthFireOpened e => Guid.Parse(e.ShipId.Value),
        EarthFireCeased e => Guid.Parse(e.ShipId.Value),
        EarthNoCannonReady e => Guid.Parse(e.ShipId.Value),
        EarthCannonJammed e => Guid.Parse(e.ShipId.Value),
        EarthCannonRepaired e => Guid.Parse(e.ShipId.Value),
        EarthCannonEmpty e => Guid.Parse(e.ShipId.Value),
        EarthShipDestroyed e => Guid.Parse(e.ShipId.Value),
        EarthShipLanded e => Guid.Parse(e.ShipId.Value),
        _ => Guid.Empty
    };

    /// <summary>Quello che esce sul bus, e quello che resta dentro la Terra: qui come nel gioco vero.</summary>
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
            EarthNoCannonReady e => new C.NoCannonReady(ContractsEarth, ship(e.ShipId), Guid.Parse(e.ShipId.Value)),
            EarthCannonJammed e => new C.CannonJammed(ContractsEarth, city(e.CityId), ship(e.ShipId),
                Guid.Parse(e.ShipId.Value)),
            EarthCannonRepaired e => new C.CannonRepaired(ContractsEarth, city(e.CityId), ship(e.ShipId),
                e.RoundsLeft, Guid.Parse(e.ShipId.Value)),
            EarthCannonEmpty e => new C.CannonEmpty(ContractsEarth, city(e.CityId), ship(e.ShipId),
                Guid.Parse(e.ShipId.Value)),
            EarthShipDestroyed e => new C.ShipDestroyed(ContractsEarth, ship(e.ShipId), city(e.CityId),
                Guid.Parse(e.ShipId.Value)),
            EarthShipLanded e => new C.ShipLanded(ContractsEarth, ship(e.ShipId), city(e.CityId), e.Damage,
                e.IntegrityLeft, Guid.Parse(e.ShipId.Value)),
            _ => null
        };
    }

    private void Deliver(Guid shipId, Event @event)
    {
        if (!_states.TryGetValue(shipId, out var state))
            return;

        Execute(_process.React(state, @event));
    }

    private void Execute(InterceptionReaction reaction)
    {
        foreach (var order in reaction.Orders)
            Apply(order);
    }

    /// <summary>
    /// Manda un ordine alla Terra, o lo lascia cadere.
    /// </summary>
    /// <remarks>
    /// La perdita sta qui e non nell'aggregato, come nel gioco vero: un ordine perso non arriva
    /// proprio, quindi non produce nessun evento e non c'e' niente da raccontare.
    /// </remarks>
    private void Apply(Command order)
    {
        if (order is OpenFire or CeaseFire or RepairCannon && !Radio.Delivers(++_orders))
        {
            _lost++;
            return;
        }

        switch (order)
        {
            case OpenFire fire:
                _earth.OpenFire(new EarthShipId(Guid.Parse(fire.ShipId.Value)), Guid.NewGuid());
                break;
            case CeaseFire cease when Compensates:
                _earth.CeaseFire(new EarthCityId(Guid.Parse(cease.CityId.Value)),
                    new EarthShipId(Guid.Parse(cease.ShipId.Value)), Guid.NewGuid());
                break;
            case RepairCannon repair:
                _earth.RepairCannon(new EarthCityId(Guid.Parse(repair.CityId.Value)),
                    new EarthShipId(Guid.Parse(repair.ShipId.Value)), Guid.NewGuid());
                break;
            default:
                return;
        }

        Drain();
    }

    private List<Guid> Standing() =>
        _earth.Cannons.Where(c => !c.Value.Fallen).Select(c => Guid.Parse(c.Key)).ToList();
}
