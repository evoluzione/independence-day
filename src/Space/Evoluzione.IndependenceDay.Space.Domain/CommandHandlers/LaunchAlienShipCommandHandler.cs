using Evoluzione.IndependenceDay.Space.Domain.Entities;

namespace Evoluzione.IndependenceDay.Space.Domain.CommandHandlers;

public class LaunchAlienShipCommandHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerAsync<LaunchAlienShip>(repository, loggerFactory)
{
    public override async Task HandleAsync(LaunchAlienShip command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var fleet = AlienShip.Launch(
            command.ShipId,
            command.TargetCity,
            command.MotherShip,
            command.ShipClass,
            command.Wave,
            command.CorrelationId);

        await Repository.SaveAsync(fleet, command.MessageId, cancellationToken);
    }
}
