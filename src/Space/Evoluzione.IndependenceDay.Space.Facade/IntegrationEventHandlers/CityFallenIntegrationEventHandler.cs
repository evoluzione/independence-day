using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Infrastructure.Messaging;
using Evoluzione.IndependenceDay.Space.ReadModel;

namespace Evoluzione.IndependenceDay.Space.Facade.IntegrationEventHandlers;

/// <summary>
/// Una citta' e' caduta: lo Spazio smette di puntarla.
/// </summary>
/// <remarks>
/// Senza questo, una buona fetta delle navi finiva su macerie — colpi a vuoto che allungavano la
/// coda della partita e rendevano piu' facili i livelli alti.
/// </remarks>
public class CityFallenIntegrationEventHandler(ITargetCityService cities, ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<CityFallen>(loggerFactory)
{
    public override Task HandleAsync(CityFallen @event, CancellationToken cancellationToken = default) =>
        cities.MarkFallen(Guid.Parse(@event.CityId.Value), @event.When(), cancellationToken);
}
