using Evoluzione.IndependenceDay.Contracts.World;
using Muflone.Core;

namespace Evoluzione.IndependenceDay.Earth.Domain.Entities;

public enum CannonStatus
{
    Ready = 0,

    Firing = 1,

    Jammed = 2,

    Empty = 3,

    Resupplying = 4,

    Lost = 5
}

public sealed class Cannon
{
    public string Name { get; set; } = string.Empty;
    public int Integrity { get; set; }
    public int Rounds { get; set; }
    public CannonStatus Status { get; set; }

    public string? Target { get; set; }

    public int Attempts { get; set; }

    public bool Fallen => Integrity <= 0;

    public bool Available => Status == CannonStatus.Ready && Rounds > 0;
}

public sealed record Incoming(string CityId, ShipClass Class, int Hits);

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

        if (wave == LastRecommissionedWave)
            return;

        RaiseEvent(new EarthRecommissioned((EarthId)Id, rounds, wave, integrity, correlationId));
    }

    public void DetectShip(CityId cityId, ShipId shipId, ShipClass shipClass, Guid correlationId)
    {

        if (Ships.ContainsKey(shipId.Value) || !Cannons.TryGetValue(cityId.Value, out var city) || city.Fallen)
            return;

        RaiseEvent(new EarthShipDetected((EarthId)Id, cityId, shipId, shipClass, correlationId));
    }

    public void OpenFire(ShipId shipId, Guid correlationId)
    {

        if (!Ships.ContainsKey(shipId.Value))
            return;

        if (Cannons.Values.Any(cannon => cannon.Target == shipId.Value))
            return;

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

    public void CeaseFire(ShipId shipId, Guid correlationId)
    {
        var engaged = Cannons
            .Where(entry => entry.Value.Status == CannonStatus.Firing && entry.Value.Target == shipId.Value)
            .Select(entry => (City: new CityId(Guid.Parse(entry.Key)), entry.Value.Rounds))
            .ToList();

        foreach (var (city, rounds) in engaged)
            RaiseEvent(new EarthFireCeased((EarthId)Id, city, shipId, rounds, correlationId));
    }

    public void RepairCannon(CityId cityId, ShipId shipId, Guid correlationId)
    {
        if (!Cannons.TryGetValue(cityId.Value, out var cannon) || cannon.Status != CannonStatus.Jammed)
            return;

        var left = Math.Max(0, cannon.Rounds - Armory.RepairCost);

        RaiseEvent(new EarthCannonRepaired((EarthId)Id, cityId, shipId, left, correlationId));

        if (left <= 0)
            RaiseEvent(new EarthCannonEmpty((EarthId)Id, cityId, shipId, correlationId));
    }

    public void RequestResupply(CityId cityId, ShipId shipId, Guid correlationId)
    {
        if (!Cannons.TryGetValue(cityId.Value, out var cannon) || cannon.Status != CannonStatus.Empty)
            return;

        RaiseEvent(new EarthResupplyDispatched((EarthId)Id, cityId, shipId, correlationId));
    }

    public void DeliverSupplies(CityId cityId, ShipId shipId, Guid correlationId)
    {
        if (!Cannons.TryGetValue(cityId.Value, out var cannon) || cannon.Status != CannonStatus.Resupplying)
            return;

        RaiseEvent(new EarthCannonResupplied((EarthId)Id, cityId, shipId, Armory.ResupplyRounds, correlationId));
    }

    public void PullTrigger(CityId cityId, Guid correlationId)
    {
        if (!Cannons.TryGetValue(cityId.Value, out var cannon) ||
            cannon.Status != CannonStatus.Firing ||
            cannon.Target is null)
            return;

        var target = new ShipId(Guid.Parse(cannon.Target));

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

    public void LandShip(ShipId shipId, Guid correlationId)
    {
        if (!Ships.TryGetValue(shipId.Value, out var ship))
            return;

        var cannon = Cannons[ship.CityId];
        var cityId = new CityId(Guid.Parse(ship.CityId));

        if (cannon.Fallen)
        {
            RaiseEvent(new EarthShipLanded((EarthId)Id, cityId, shipId, 0, 0, correlationId));
            return;
        }

        var damage = Contracts.World.Ships.DamageOf(ship.Class);
        var integrityLeft = Math.Max(0, cannon.Integrity - damage);

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
            cannon.Attempts = 0;
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
        cannon.Status = CannonStatus.Jammed;
    }

    public void Apply(EarthCannonRepaired @event)
    {
        var cannon = Cannons[@event.CityId.Value];
        cannon.Rounds = @event.RoundsLeft;
        cannon.Status = CannonStatus.Ready;
    }

    public void Apply(EarthCannonEmpty @event)
    {
        var cannon = Cannons[@event.CityId.Value];
        cannon.Rounds = 0;
        cannon.Target = null;
        cannon.Status = CannonStatus.Empty;
    }

    public void Apply(EarthResupplyDispatched @event) =>
        Cannons[@event.CityId.Value].Status = CannonStatus.Resupplying;

    public void Apply(EarthCannonResupplied @event)
    {
        var cannon = Cannons[@event.CityId.Value];
        cannon.Rounds = @event.Rounds;
        cannon.Status = CannonStatus.Ready;
    }

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

    public void Apply(EarthCityFallen @event)
    {
        var cannon = Cannons[@event.CityId.Value];
        cannon.Integrity = 0;
        cannon.Target = null;
        cannon.Status = CannonStatus.Lost;
    }
}
