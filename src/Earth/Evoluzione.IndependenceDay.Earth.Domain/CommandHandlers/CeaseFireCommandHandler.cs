using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Infrastructure.Persistence;

namespace Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;

public class CeaseFireCommandHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerAsync<CeaseFire>(repository, loggerFactory)
{
    public override Task HandleAsync(CeaseFire command, CancellationToken cancellationToken = default) =>
        FireOrders.On(Repository, (Contracts.Ids.EarthId)command.AggregateId,
            earth => earth.CeaseFire(FireOrders.City(command.CityId), FireOrders.Ship(command.ShipId),
                command.CorrelationId),
            command.MessageId, cancellationToken);
}
