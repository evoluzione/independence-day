using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

/// <summary>
/// Un caccia al primo livello rade al suolo una citta' esattamente come una corazzata al decimo.
/// </summary>
/// <remarks>
/// Non c'e' integrita' da erodere: o la nave viene fermata, o quella citta' non c'e' piu'. E' la
/// regola che rende il non fare niente una sconfitta immediata invece di un costo da ammortizzare —
/// e il prezzo non e' un punto, e' il cannone di quella citta' per tutto il resto della campagna.
/// </remarks>
public class LandShip_WhenAnyShipTouchesGround_RazesTheCity : EarthCommandSpecification<LandShip>
{
    private readonly ShipId _shipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        // La stazza piu' leggera che ci sia: quella che verrebbe voglia di lasciar passare.
        yield return new EarthShipDetected(Earth, City, _shipId, ShipClass.Fighter, Guid.NewGuid());
    }

    protected override LandShip When() => new(Earth, _shipId, _correlationId, Coordinator);

    protected override ICommandHandlerAsync<LandShip> OnHandler() =>
        new LandShipCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new EarthShipLanded(Earth, City, _shipId, Ships.FullIntegrity, 0, _correlationId);
        yield return new EarthCityFallen(Earth, City, _shipId, _correlationId);
    }
}
