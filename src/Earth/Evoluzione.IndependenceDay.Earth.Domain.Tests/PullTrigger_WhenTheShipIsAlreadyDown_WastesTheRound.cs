using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

/// <summary>
/// La nave e' gia' caduta e il cannone spara ancora: il colpo si perde.
/// </summary>
/// <remarks>
/// La Terra <b>non</b> spegne il cannone da sola, e non e' una svista. Non e' lei a sapere se quel
/// fuoco serviva ancora: lo sa chi l'ha aperto, ed e' lui che deve chiuderlo. Ogni riga come questa
/// nel diario e' un cessate il fuoco che non e' mai arrivato — un colpo in meno adesso, e un cannone
/// che non c'e' quando arriva la nave dopo.
/// </remarks>
public class PullTrigger_WhenTheShipIsAlreadyDown_WastesTheRound : EarthCommandSpecification<PullTrigger>
{
    private readonly ShipId _shipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        yield return new EarthShipDetected(Earth, City, _shipId, ShipClass.Fighter, Guid.NewGuid());
        yield return new EarthFireOpened(Earth, City, _shipId, Armory.RoundsPerCity, Guid.NewGuid());
        yield return new EarthShotFired(Earth, City, _shipId, 1, Armory.RoundsPerCity - 1, Guid.NewGuid());
        yield return new EarthShipDestroyed(Earth, City, _shipId, Guid.NewGuid());
    }

    protected override PullTrigger When() => new(Earth, City, _correlationId, Coordinator);

    protected override ICommandHandlerAsync<PullTrigger> OnHandler() =>
        new PullTriggerCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new EarthShotWasted(Earth, City, _shipId, Armory.RoundsPerCity - 2, _correlationId);
    }
}
