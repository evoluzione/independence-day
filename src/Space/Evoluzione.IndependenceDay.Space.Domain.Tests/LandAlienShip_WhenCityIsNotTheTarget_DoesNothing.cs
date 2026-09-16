using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Space.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Space.Messages.Commands;
using Evoluzione.IndependenceDay.Space.Messages.DomainIds;
using Evoluzione.IndependenceDay.Space.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;

namespace Evoluzione.IndependenceDay.Space.Domain.Tests;

/// <summary>
/// Una nave atterra sulla citta' che puntava. Un ordine per un'altra citta' e' un messaggio finito
/// sull'aggregato sbagliato: si scarta in silenzio invece di inventare un atterraggio.
/// </summary>
public class LandAlienShip_WhenCityIsNotTheTarget_DoesNothing : SpaceCommandSpecification<LandAlienShip>
{
    private readonly ShipId _shipId = new(Guid.NewGuid());
    private readonly CityId _targetCity = new(Guid.NewGuid());
    private readonly CityId _anotherCity = new(Guid.NewGuid());
    private readonly MotherShipId _motherShipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        yield return new AlienShipLaunched(_shipId, _targetCity, _motherShipId, ShipClass.Cruiser, Wave, _correlationId);
    }

    protected override LandAlienShip When() => new(_shipId, _anotherCity, _correlationId, Invaders);

    protected override ICommandHandlerAsync<LandAlienShip> OnHandler() =>
        new LandAlienShipCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect() => [];
}
