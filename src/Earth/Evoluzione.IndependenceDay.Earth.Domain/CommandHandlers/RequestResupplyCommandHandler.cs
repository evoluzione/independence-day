using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Infrastructure.Persistence;

namespace Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;

public class RequestResupplyCommandHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerAsync<RequestResupply>(repository, loggerFactory)
{
    public override Task HandleAsync(RequestResupply command, CancellationToken cancellationToken = default) =>
        FireOrders.On(Repository, (Contracts.Ids.EarthId)command.AggregateId,
            earth => earth.RequestResupply(FireOrders.City(command.CityId), FireOrders.Ship(command.ShipId),
                command.CorrelationId),
            command.MessageId, cancellationToken);
}
