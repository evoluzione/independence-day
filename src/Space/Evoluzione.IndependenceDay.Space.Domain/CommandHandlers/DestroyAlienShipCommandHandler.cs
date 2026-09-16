using Evoluzione.IndependenceDay.Infrastructure.Persistence;
using Evoluzione.IndependenceDay.Space.Domain.Entities;

namespace Evoluzione.IndependenceDay.Space.Domain.CommandHandlers;

public class DestroyAlienShipCommandHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerAsync<DestroyAlienShip>(repository, loggerFactory)
{
    public override async Task HandleAsync(DestroyAlienShip command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var fleet = await Repository.TryGetByIdAsync<AlienShip>(command.ShipId, cancellationToken);
        if (fleet is null)
            return;

        fleet.Destroy(command.Cause, command.CorrelationId);

        await Repository.SaveAsync(fleet, command.MessageId, cancellationToken);
    }
}
