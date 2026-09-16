using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

/// <summary>
/// Un incrociatore regge tre colpi. Al terzo cade — e i cannoni continuano a sparare.
/// </summary>
/// <remarks>
/// Quanti colpi serva a una stazza <b>non</b> lo dice nessun evento: chi coordina lo scopre perche'
/// la nave cade, non perche' l'ha calcolato. E' una scelta di progetto: il conto e' una decisione di
/// dominio, e resta qui.
/// </remarks>
public class PullTrigger_WhenTheLastHitLands_DestroysTheShip : EarthCommandSpecification<PullTrigger>
{
    private readonly ShipId _shipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        yield return new EarthShipDetected(Earth, City, _shipId, ShipClass.Cruiser, Guid.NewGuid());
        yield return new EarthFireOpened(Earth, City, _shipId, Armory.RoundsPerCity, Guid.NewGuid());
        yield return new EarthShotFired(Earth, City, _shipId, 1, Armory.RoundsPerCity - 1, Guid.NewGuid());
        yield return new EarthShotFired(Earth, City, _shipId, 2, Armory.RoundsPerCity - 2, Guid.NewGuid());
    }

    protected override PullTrigger When() => new(Earth, City, _correlationId, Coordinator);

    protected override ICommandHandlerAsync<PullTrigger> OnHandler() =>
        new PullTriggerCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new EarthShotFired(Earth, City, _shipId, 3, Armory.RoundsPerCity - 3, _correlationId);
        yield return new EarthShipDestroyed(Earth, City, _shipId, _correlationId);
    }
}
