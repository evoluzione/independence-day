using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

/// <summary>
/// Una riparazione su due non prende: i colpi se ne vanno e il cannone resta inceppato.
/// </summary>
/// <remarks>
/// Non e' un rifiuto e non e' un errore — la Terra lo racconta, e chi ha ordinato la riparazione lo
/// scopre da li'. Da fuori e' indistinguibile da un ordine perso per strada, e non serve
/// distinguerlo: in tutti e due i casi si insiste.
/// <para>
/// Il conto e' sulle riparazioni di quel cannone: qui la prima ha gia' preso e il cannone si e'
/// inceppato di nuovo, quindi questo e' il secondo tentativo, ed e' quello che non prende.
/// </para>
/// </remarks>
public class RepairCannon_WhenTheAttemptDoesNotTake_SpendsTheRoundsAnyway
    : EarthCommandSpecification<RepairCannon>
{
    private const int AfterFirstRepair = Armory.RoundsPerCity - Armory.RepairCost;

    private readonly ShipId _shipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        yield return new EarthShipDetected(Earth, City, _shipId, ShipClass.Battleship, Guid.NewGuid());
        yield return new EarthFireOpened(Earth, City, _shipId, Armory.RoundsPerCity, Guid.NewGuid());

        // Primo inceppamento e prima riparazione: quella prende sempre.
        yield return new EarthCannonJammed(Earth, City, _shipId, Guid.NewGuid());
        yield return new EarthCannonRepaired(Earth, City, _shipId, AfterFirstRepair, Guid.NewGuid());

        // E il cannone si inceppa di nuovo.
        yield return new EarthCannonJammed(Earth, City, _shipId, Guid.NewGuid());
    }

    protected override RepairCannon When() =>
        new(SharedEarth, SharedCity, SharedShip(_shipId), _correlationId, Coordinator);

    protected override ICommandHandlerAsync<RepairCannon> OnHandler() =>
        new RepairCannonCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new EarthCannonStillJammed(Earth, City, _shipId, AfterFirstRepair - Armory.RepairCost,
            _correlationId);
    }
}
