using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using ShipId = Evoluzione.IndependenceDay.Earth.Messages.DomainIds.ShipId;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

/// <summary>
/// L'ordine perso non produce niente che esca dalla Terra.
/// </summary>
/// <remarks>
/// La traccia resta nel diario, ma non esiste nessuna versione di integrazione di questo evento:
/// chi ha ordinato non riceve <b>niente</b>, e se ne accorge solo al battito successivo. E' il
/// guasto piu' scomodo dei tre, perche' non c'e' un esito da aspettare — c'e' solo un'assenza.
/// </remarks>
public class OpenFire_WhenTheOrderGetsLost_LeavesOnlySilence : EarthCommandSpecification<OpenFire>
{
    private readonly ShipId _shipId = new(Guid.NewGuid());
    private readonly Guid _correlationId = Guid.NewGuid();

    protected override IEnumerable<DomainEvent> Given()
    {
        foreach (var e in EarthStanding())
            yield return e;

        // Due ordini gia' passati: il prossimo e' quello che si perde.
        foreach (var e in OrdersSoFar(2))
            yield return e;

        yield return new EarthShipDetected(Earth, City, _shipId, ShipClass.Fighter, Guid.NewGuid());
    }

    protected override OpenFire When() => new(SharedEarth, SharedShip(_shipId), _correlationId, Coordinator);

    protected override ICommandHandlerAsync<OpenFire> OnHandler() =>
        new OpenFireCommandHandler(Repository, LoggerFactory);

    protected override IEnumerable<DomainEvent> Expect()
    {
        yield return new EarthOrderLost(Earth, "apertura del fuoco", City, _shipId, _correlationId);
    }
}
