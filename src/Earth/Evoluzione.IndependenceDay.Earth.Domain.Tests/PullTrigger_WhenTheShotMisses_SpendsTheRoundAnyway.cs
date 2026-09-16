using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

/// <summary>
/// Un colpo su tre manca il bersaglio: la munizione se ne va e la nave regge.
/// </summary>
/// <remarks>
/// Non c'e' niente da fare e non arriva niente a chi coordina: il fuoco e' aperto, quindi il cannone
/// ricarica e riprova da solo. Il prezzo non e' la munizione, e' il tempo — quel cannone resta
/// occupato piu' a lungo, e i cannoni sono la cosa che scarseggia.
/// </remarks>
public class PullTrigger_WhenTheShotMisses_SpendsTheRoundAnyway : EarthCommandSpecification<PullTrigger>
{
    private readonly ShipId _shipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        yield return new EarthShipDetected(Earth, City, _shipId, ShipClass.Battleship, Guid.NewGuid());
        yield return new EarthFireOpened(Earth, City, _shipId, Armory.RoundsPerCity, Guid.NewGuid());

        // Due colpi a segno: il terzo e' quello che manca il bersaglio.
        yield return new EarthShotFired(Earth, City, _shipId, 1, Armory.RoundsPerCity - 1, Guid.NewGuid());
        yield return new EarthShotFired(Earth, City, _shipId, 2, Armory.RoundsPerCity - 2, Guid.NewGuid());
    }

    protected override PullTrigger When() => new(Earth, City, _correlationId, Coordinator);

    protected override ICommandHandlerAsync<PullTrigger> OnHandler() =>
        new PullTriggerCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new EarthShotMissed(Earth, City, _shipId, Armory.RoundsPerCity - 3, _correlationId);
    }
}
