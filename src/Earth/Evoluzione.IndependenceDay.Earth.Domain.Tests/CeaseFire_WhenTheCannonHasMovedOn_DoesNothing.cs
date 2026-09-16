using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

/// <summary>
/// Un cessate il fuoco in ritardo non spegne il cannone che nel frattempo e' stato messo altrove.
/// </summary>
/// <remarks>
/// Il bersaglio fa parte dell'ordine proprio per questo. Senza, un processo che chiude in ritardo
/// zittirebbe il cannone di un altro — e sarebbe un guasto che nessuno riesce a spiegarsi guardando
/// il proprio pezzo di storia.
/// </remarks>
public class CeaseFire_WhenTheCannonHasMovedOn_DoesNothing : EarthCommandSpecification<CeaseFire>
{
    private readonly ShipId _old = new(Guid.NewGuid());
    private readonly ShipId _current = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        yield return new EarthShipDetected(Earth, City, _current, ShipClass.Fighter, Guid.NewGuid());
        yield return new EarthFireOpened(Earth, City, _current, Armory.RoundsPerCity, Guid.NewGuid());
    }

    protected override CeaseFire When() => new(SharedEarth, SharedCity, SharedShip(_old), _correlationId, Coordinator);

    protected override ICommandHandlerAsync<CeaseFire> OnHandler() =>
        new CeaseFireCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect() => [];
}
