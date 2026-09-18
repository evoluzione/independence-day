using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Infrastructure.Messaging;
using Evoluzione.IndependenceDay.Space.ReadModel;

namespace Evoluzione.IndependenceDay.Space.Facade.IntegrationEventHandlers;

public class CityFallenIntegrationEventHandler(ITargetCityService cities, ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<CityFallen>(loggerFactory)
{
    public override Task HandleAsync(CityFallen @event, CancellationToken cancellationToken = default) =>
        cities.MarkFallen(Guid.Parse(@event.CityId.Value), @event.When(), cancellationToken);
}
