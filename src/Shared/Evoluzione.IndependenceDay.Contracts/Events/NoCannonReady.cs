namespace Evoluzione.IndependenceDay.Contracts.Events;

/// <summary>
/// Nessun cannone disponibile per quella nave.
/// </summary>
/// <remarks>
/// Tutti stanno sparando a qualcos'altro, sono inceppati o sono a secco. Non e' un errore ed e' un
/// esito temporaneo: basta che un cannone cessi il fuoco perche' torni a essercene uno. Chi coordina
/// deve riprovare, non arrendersi — ma finche' non riprova, quella nave non la sta fermando nessuno.
/// </remarks>
public sealed class NoCannonReady(EarthId aggregateId, ShipId shipId, Guid correlationId)
    : IntegrationEvent(aggregateId, correlationId)
{
    public ShipId ShipId { get; } = shipId;
}
