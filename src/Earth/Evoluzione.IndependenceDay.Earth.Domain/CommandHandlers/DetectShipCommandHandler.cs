using Evoluzione.IndependenceDay.Earth.Domain.Entities;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Infrastructure.Persistence;

namespace Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;

public class DetectShipCommandHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerAsync<DetectShip>(repository, loggerFactory)
{
    public override async Task HandleAsync(DetectShip command, CancellationToken cancellationToken = default)
    {
        var earth = await Repository.TryGetByIdAsync<EarthDefense>((EarthId)command.AggregateId, cancellationToken);
        if (earth is null)
            return;

        earth.DetectShip(command.CityId, command.ShipId, command.ShipClass, command.CorrelationId);

        await Repository.SaveAsync(earth, command.MessageId, cancellationToken);
    }
}
