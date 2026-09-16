using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Infrastructure.Persistence;

namespace Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;

public class OpenFireCommandHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerAsync<OpenFire>(repository, loggerFactory)
{
    public override Task HandleAsync(OpenFire command, CancellationToken cancellationToken = default) =>
        FireOrders.On(Repository, (Contracts.Ids.EarthId)command.AggregateId,
            earth => earth.OpenFire(FireOrders.Ship(command.ShipId), command.CorrelationId),
            command.MessageId, cancellationToken);
}
