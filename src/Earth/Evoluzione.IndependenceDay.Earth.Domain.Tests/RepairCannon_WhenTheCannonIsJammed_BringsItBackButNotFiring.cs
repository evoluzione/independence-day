using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

/// <summary>
/// Riparare rimette il cannone in piedi, non in azione.
/// </summary>
/// <remarks>
/// Torna disponibile, con qualche colpo in meno, e fermo. Chi lo ha fatto riparare deve anche
/// rimetterlo a sparare: fermarsi a meta' significa aver speso munizioni per un cannone che resta
/// li' a guardare.
/// </remarks>
public class RepairCannon_WhenTheCannonIsJammed_BringsItBackButNotFiring : EarthCommandSpecification<RepairCannon>
{
    private readonly ShipId _shipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        yield return new EarthShipDetected(Earth, City, _shipId, ShipClass.Cruiser, Guid.NewGuid());
        yield return new EarthFireOpened(Earth, City, _shipId, Armory.RoundsPerCity, Guid.NewGuid());
        yield return new EarthCannonJammed(Earth, City, _shipId, Guid.NewGuid());
    }

    protected override RepairCannon When() => new(SharedEarth, SharedCity, SharedShip(_shipId), _correlationId, Coordinator);

    protected override ICommandHandlerAsync<RepairCannon> OnHandler() =>
        new RepairCannonCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new EarthCannonRepaired(Earth, City, _shipId, Armory.RoundsPerCity - Armory.RepairCost,
            _correlationId);
    }
}
