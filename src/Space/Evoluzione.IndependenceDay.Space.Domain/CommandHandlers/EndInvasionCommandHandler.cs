using Evoluzione.IndependenceDay.Infrastructure.Persistence;
using Evoluzione.IndependenceDay.Space.Domain.Entities;

namespace Evoluzione.IndependenceDay.Space.Domain.CommandHandlers;

public class EndInvasionCommandHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerAsync<EndInvasion>(repository, loggerFactory)
{
    public override async Task HandleAsync(EndInvasion command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var invasion = await Repository.TryGetByIdAsync<Invasion>(command.InvasionId, cancellationToken);
        if (invasion is null)
            return;

        invasion.EndWave(command.Wave, command.CorrelationId);

        await Repository.SaveAsync(invasion, command.MessageId, cancellationToken);
    }
}
