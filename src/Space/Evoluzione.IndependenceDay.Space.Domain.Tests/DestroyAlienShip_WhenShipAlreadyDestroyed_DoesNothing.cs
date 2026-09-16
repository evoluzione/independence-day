using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Space.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Space.Messages.Commands;
using Evoluzione.IndependenceDay.Space.Messages.DomainIds;
using Evoluzione.IndependenceDay.Space.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;

namespace Evoluzione.IndependenceDay.Space.Domain.Tests;

/// <summary>
/// Il caso della riconsegna: respinta e atomica possono arrivare tutte e due sulla stessa nave.
/// La seconda non e' un errore e non deve scrivere niente.
/// </summary>
public class DestroyAlienShip_WhenShipAlreadyDestroyed_DoesNothing : SpaceCommandSpecification<DestroyAlienShip>
{
    private readonly ShipId _shipId = new(Guid.NewGuid());
    private readonly CityId _cityId = new(Guid.NewGuid());
    private readonly MotherShipId _motherShipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        yield return new AlienShipLaunched(_shipId, _cityId, _motherShipId, ShipClass.Cruiser, Wave, _correlationId);
        yield return new AlienShipDestroyed(_shipId, "repelled", _correlationId);
    }

    protected override DestroyAlienShip When() => new(_shipId, "nuked", _correlationId, Invaders);

    protected override ICommandHandlerAsync<DestroyAlienShip> OnHandler() =>
        new DestroyAlienShipCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect() => [];
}
