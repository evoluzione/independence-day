using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Space.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Space.Messages.Commands;
using Evoluzione.IndependenceDay.Space.Messages.DomainIds;
using Evoluzione.IndependenceDay.Space.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;

namespace Evoluzione.IndependenceDay.Space.Domain.Tests;

public class LaunchAlienShip_WhenDataAreValid_LaunchesTheShip : SpaceCommandSpecification<LaunchAlienShip>
{
    private readonly ShipId _shipId = new(Guid.NewGuid());
    private readonly CityId _cityId = new(Guid.NewGuid());
    private readonly MotherShipId _motherShipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given() => [];

    protected override LaunchAlienShip When() =>
        new(_shipId, _cityId, _motherShipId, ShipClass.Cruiser, Wave, _correlationId, Invaders);

    protected override ICommandHandlerAsync<LaunchAlienShip> OnHandler() =>
        new LaunchAlienShipCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new AlienShipLaunched(_shipId, _cityId, _motherShipId, ShipClass.Cruiser, Wave, _correlationId);
    }
}
