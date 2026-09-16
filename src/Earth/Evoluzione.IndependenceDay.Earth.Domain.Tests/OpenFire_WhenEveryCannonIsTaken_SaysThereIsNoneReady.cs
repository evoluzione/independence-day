using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using CityId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.CityId;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

/// <summary>
/// Cinque cannoni gia' impegnati e una sesta nave: non c'e' niente da mandare.
/// </summary>
/// <remarks>
/// Non e' un errore ed e' reversibile: basta che un cannone cessi il fuoco perche' torni a
/// essercene uno. Ma finche' nessuno lo fa, quella nave non la sta fermando nessuno — ed e' questa
/// la ragione per cui il cessate il fuoco vale quanto l'apertura.
/// </remarks>
public class OpenFire_WhenEveryCannonIsTaken_SaysThereIsNoneReady : EarthCommandSpecification<OpenFire>
{
    private readonly ShipId _sixth = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        // Cinque navi gia' prese in carico, una per cannone, e tutti e cinque i cannoni aperti.
        foreach (var city in Cities.All)
        {
            var busy = new ShipId(Guid.NewGuid());
            var cityId = new CityId(city.Id);

            yield return new EarthShipDetected(Earth, cityId, busy, ShipClass.Fighter, Guid.NewGuid());
            yield return new EarthFireOpened(Earth, cityId, busy, Armory.RoundsPerCity, Guid.NewGuid());
        }

        yield return new EarthShipDetected(Earth, City, _sixth, ShipClass.Battleship, Guid.NewGuid());
    }

    protected override OpenFire When() => new(SharedEarth, SharedShip(_sixth), _correlationId, Coordinator);

    protected override ICommandHandlerAsync<OpenFire> OnHandler() =>
        new OpenFireCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new EarthNoCannonReady(Earth, _sixth, _correlationId);
    }
}
