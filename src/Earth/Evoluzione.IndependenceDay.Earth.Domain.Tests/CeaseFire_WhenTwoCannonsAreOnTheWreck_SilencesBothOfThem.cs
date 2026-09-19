using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using CityId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.CityId;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

public class CeaseFire_WhenTwoCannonsAreOnTheWreck_SilencesBothOfThem : EarthCommandSpecification<CeaseFire>
{
    private static readonly CityId Second = new(Cities.All[1].Id);

    private readonly ShipId _ship = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        yield return new EarthShipDetected(Earth, City, _ship, ShipClass.Battleship, Guid.NewGuid());
        yield return new EarthFireOpened(Earth, City, _ship, Armory.RoundsPerCity, Guid.NewGuid());
        yield return new EarthFireOpened(Earth, Second, _ship, Armory.RoundsPerCity, Guid.NewGuid());
    }

    protected override CeaseFire When() => new(SharedEarth, SharedShip(_ship), _correlationId, Coordinator);

    protected override ICommandHandlerAsync<CeaseFire> OnHandler() =>
        new CeaseFireCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new EarthFireCeased(Earth, City, _ship, Armory.RoundsPerCity, _correlationId);
        yield return new EarthFireCeased(Earth, Second, _ship, Armory.RoundsPerCity, _correlationId);
    }
}
