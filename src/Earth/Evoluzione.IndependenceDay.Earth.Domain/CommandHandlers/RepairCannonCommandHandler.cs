using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Infrastructure.Persistence;

namespace Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;

public class RepairCannonCommandHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerAsync<RepairCannon>(repository, loggerFactory)
{
    public override Task HandleAsync(RepairCannon command, CancellationToken cancellationToken = default) =>
        FireOrders.On(Repository, (Contracts.Ids.EarthId)command.AggregateId,
            earth => earth.RepairCannon(FireOrders.City(command.CityId), FireOrders.Ship(command.ShipId),
                command.CorrelationId),
            command.MessageId, cancellationToken);
}
