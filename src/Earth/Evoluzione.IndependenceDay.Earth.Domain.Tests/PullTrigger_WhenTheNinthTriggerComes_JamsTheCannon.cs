using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

/// <summary>
/// Un cannone si inceppa ogni nove grilletti, e si porta dietro il bersaglio.
/// </summary>
/// <remarks>
/// Il colpo non parte e non consuma niente, ma il cannone resta fermo e la nave che stava
/// affrontando si ritrova senza nessuno addosso. Non si sblocca da solo: se nessuno lo ripara, e'
/// perso per il resto della campagna.
/// </remarks>
public class PullTrigger_WhenTheNinthTriggerComes_JamsTheCannon : EarthCommandSpecification<PullTrigger>
{
    private readonly ShipId _shipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        yield return new EarthShipDetected(Earth, City, _shipId, ShipClass.Battleship, Guid.NewGuid());
        yield return new EarthFireOpened(Earth, City, _shipId, Armory.RoundsPerCity, Guid.NewGuid());

        // Otto grilletti gia' premuti: il nono e' quello che si inceppa.
        for (var shot = 1; shot <= Armory.JamEveryShots - 1; shot++)
            yield return new EarthShotFired(Earth, City, _shipId, shot, Armory.RoundsPerCity - shot,
                Guid.NewGuid());
    }

    protected override PullTrigger When() => new(Earth, City, _correlationId, Coordinator);

    protected override ICommandHandlerAsync<PullTrigger> OnHandler() =>
        new PullTriggerCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new EarthCannonJammed(Earth, City, _shipId, _correlationId);
    }
}
