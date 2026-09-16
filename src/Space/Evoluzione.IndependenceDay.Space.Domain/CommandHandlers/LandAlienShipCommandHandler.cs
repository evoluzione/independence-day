using Evoluzione.IndependenceDay.Infrastructure.Persistence;
using Evoluzione.IndependenceDay.Space.Domain.Entities;

namespace Evoluzione.IndependenceDay.Space.Domain.CommandHandlers;

public class LandAlienShipCommandHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerAsync<LandAlienShip>(repository, loggerFactory)
{
    public override async Task HandleAsync(LandAlienShip command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var fleet = await Repository.TryGetByIdAsync<AlienShip>(command.ShipId, cancellationToken);
        if (fleet is null)
            return;

        fleet.Land(command.CityId, command.CorrelationId);

        await Repository.SaveAsync(fleet, command.MessageId, cancellationToken);
    }
}
