using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

public class RequestResupply_WhenTheCannonIsDry_SendsTheConvoy : EarthCommandSpecification<RequestResupply>
{
    private readonly ShipId _shipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        yield return new EarthShipDetected(Earth, City, _shipId, ShipClass.Battleship, Guid.NewGuid());
        yield return new EarthFireOpened(Earth, City, _shipId, Armory.RoundsPerCity, Guid.NewGuid());
        yield return new EarthCannonEmpty(Earth, City, _shipId, Guid.NewGuid());
    }

    protected override RequestResupply When() =>
        new(SharedEarth, SharedCity, SharedShip(_shipId), _correlationId, Coordinator);

    protected override ICommandHandlerAsync<RequestResupply> OnHandler() =>
        new RequestResupplyCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new EarthResupplyDispatched(Earth, City, _shipId, _correlationId);
    }
}
