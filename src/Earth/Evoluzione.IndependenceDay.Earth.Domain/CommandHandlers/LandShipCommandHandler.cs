using Evoluzione.IndependenceDay.Earth.Domain.Entities;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Infrastructure.Persistence;

namespace Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;

public class LandShipCommandHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerAsync<LandShip>(repository, loggerFactory)
{
    public override async Task HandleAsync(LandShip command, CancellationToken cancellationToken = default)
    {
        var earth = await Repository.TryGetByIdAsync<EarthDefense>((EarthId)command.AggregateId, cancellationToken);
        if (earth is null)
            return;

        earth.LandShip(command.ShipId, command.CorrelationId);

        await Repository.SaveAsync(earth, command.MessageId, cancellationToken);
    }
}
