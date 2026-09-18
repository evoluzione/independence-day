using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

public class CeaseFire_WhenTheCannonHasMovedOn_DoesNothing : EarthCommandSpecification<CeaseFire>
{
    private readonly ShipId _old = new(Guid.NewGuid());
    private readonly ShipId _current = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        yield return new EarthShipDetected(Earth, City, _current, ShipClass.Fighter, Guid.NewGuid());
        yield return new EarthFireOpened(Earth, City, _current, Armory.RoundsPerCity, Guid.NewGuid());
    }

    protected override CeaseFire When() => new(SharedEarth, SharedCity, SharedShip(_old), _correlationId, Coordinator);

    protected override ICommandHandlerAsync<CeaseFire> OnHandler() =>
        new CeaseFireCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect() => [];
}
