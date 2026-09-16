using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

/// <summary>
/// Quale cannone spari non lo sceglie chi ordina: lo sceglie la Terra, e sceglie il piu' carico.
/// </summary>
/// <remarks>
/// A parita' di colpi vince il primo in ordine di identificativo. Non e' un dettaglio estetico:
/// senza un criterio fisso due partite con le stesse mosse darebbero esiti diversi.
/// </remarks>
public class OpenFire_WhenSeveralCannonsAreFree_PicksTheFullestOne : EarthCommandSpecification<OpenFire>
{
    private readonly ShipId _shipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        yield return new EarthShipDetected(Earth, City, _shipId, ShipClass.Cruiser, Guid.NewGuid());
    }

    protected override OpenFire When() => new(SharedEarth, SharedShip(_shipId), _correlationId, Coordinator);

    protected override ICommandHandlerAsync<OpenFire> OnHandler() =>
        new OpenFireCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new EarthFireOpened(Earth, City, _shipId, Armory.RoundsPerCity, _correlationId);
    }
}
