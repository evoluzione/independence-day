using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

public class LandShip_WhenTheCityHasAlreadyFallen_DoesNotRazeItTwice : EarthCommandSpecification<LandShip>
{
    private readonly ShipId _firstShipId = new(Guid.NewGuid());
    private readonly ShipId _secondShipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        yield return new EarthShipDetected(Earth, City, _firstShipId, ShipClass.Fighter, Guid.NewGuid());
        yield return new EarthShipDetected(Earth, City, _secondShipId, ShipClass.Fighter, Guid.NewGuid());
        yield return new EarthShipLanded(Earth, City, _firstShipId, Ships.FullIntegrity, 0, Guid.NewGuid());
        yield return new EarthCityFallen(Earth, City, _firstShipId, Guid.NewGuid());
    }

    protected override LandShip When() => new(Earth, _secondShipId, _correlationId, Coordinator);

    protected override ICommandHandlerAsync<LandShip> OnHandler() =>
        new LandShipCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new EarthShipLanded(Earth, City, _secondShipId, 0, 0, _correlationId);
    }
}
