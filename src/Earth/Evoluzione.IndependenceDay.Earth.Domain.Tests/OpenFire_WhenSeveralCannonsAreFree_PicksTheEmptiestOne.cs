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
/// Quale cannone spari non lo sceglie chi ordina: lo sceglie la Terra, e sceglie il piu' scarico.
/// </summary>
/// <remarks>
/// Si consumano prima le riserve piccole e si tengono indietro quelle piene. Il prezzo e' voluto: un
/// cannone quasi finito puo' esaurirsi in mezzo a una nave, e restare assegnato a lei senza sparare.
/// <para>
/// A parita' di colpi vince il primo in ordine di identificativo. Non e' un dettaglio estetico:
/// senza un criterio fisso due partite con le stesse mosse darebbero esiti diversi.
/// </para>
/// </remarks>
public class OpenFire_WhenSeveralCannonsAreFree_PicksTheEmptiestOne : EarthCommandSpecification<OpenFire>
{
    private static readonly CityId Spent = new(Cities.All[4].Id);

    private readonly ShipId _shipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        yield return new EarthShipDetected(Earth, City, _shipId, ShipClass.Cruiser, Guid.NewGuid());

        // L'ultima citta' ha gia' speso qualcosa: e' lei che deve rispondere, non la prima.
        yield return new EarthShotWasted(Earth, Spent, _shipId, Armory.RoundsPerCity - 10, Guid.NewGuid());
    }

    protected override OpenFire When() => new(SharedEarth, SharedShip(_shipId), _correlationId, Coordinator);

    protected override ICommandHandlerAsync<OpenFire> OnHandler() =>
        new OpenFireCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new EarthFireOpened(Earth, Spent, _shipId, Armory.RoundsPerCity - 10, _correlationId);
    }
}
