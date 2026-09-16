using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

/// <summary>
/// Un cannone si inceppa a intervalli fissi di grilletti, e si porta dietro il bersaglio.
/// </summary>
/// <remarks>
/// Il colpo non parte e non consuma niente, ma il cannone resta fermo. Il conto e' sui grilletti
/// premuti, non sui colpi a segno: valgono anche quelli che hanno mancato il bersaglio e quelli sparati nel nulla.
/// Non si sblocca da solo — se nessuno lo ripara e' perso per il resto della campagna.
/// </remarks>
public class PullTrigger_WhenTheJammingTriggerComes_JamsTheCannon : EarthCommandSpecification<PullTrigger>
{
    private readonly ShipId _shipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        yield return new EarthShipDetected(Earth, City, _shipId, ShipClass.Battleship, Guid.NewGuid());
        yield return new EarthFireOpened(Earth, City, _shipId, Armory.RoundsPerCity, Guid.NewGuid());

        // La nave e' gia' caduta e il cannone spara ancora: e' il modo piu' pulito di accumulare
        // grilletti senza che il bersaglio finisca prima.
        yield return new EarthShipDestroyed(Earth, City, _shipId, Guid.NewGuid());

        for (var shot = 1; shot <= Armory.JamEveryShots - 1; shot++)
            yield return new EarthShotWasted(Earth, City, _shipId, Armory.RoundsPerCity - shot,
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
