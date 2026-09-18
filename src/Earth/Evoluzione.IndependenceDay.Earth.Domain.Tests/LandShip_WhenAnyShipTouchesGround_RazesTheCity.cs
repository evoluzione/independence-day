using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

public class LandShip_WhenAnyShipTouchesGround_RazesTheCity : EarthCommandSpecification<LandShip>
{
    private readonly ShipId _shipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        yield return new EarthShipDetected(Earth, City, _shipId, ShipClass.Fighter, Guid.NewGuid());
    }

    protected override LandShip When() => new(Earth, _shipId, _correlationId, Coordinator);

    protected override ICommandHandlerAsync<LandShip> OnHandler() =>
        new LandShipCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new EarthShipLanded(Earth, City, _shipId, Ships.FullIntegrity, 0, _correlationId);
        yield return new EarthCityFallen(Earth, City, _shipId, _correlationId);
    }
}
