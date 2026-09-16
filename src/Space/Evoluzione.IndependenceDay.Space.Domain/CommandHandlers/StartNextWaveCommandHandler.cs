using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Infrastructure.Persistence;
using Evoluzione.IndependenceDay.Space.Domain.Entities;
using Evoluzione.IndependenceDay.Space.Domain.Services;
using Microsoft.Extensions.Options;

namespace Evoluzione.IndependenceDay.Space.Domain.CommandHandlers;

public class StartNextWaveCommandHandler(
    IRepository repository,


    IOptions<WaveDifficulty> difficulty,
    ILoggerFactory loggerFactory) : CommandHandlerAsync<StartNextWave>(repository, loggerFactory)
{
    public override async Task HandleAsync(StartNextWave command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var invasion = await Repository.TryGetByIdAsync<Invasion>(
            new InvasionId(Guid.Parse(command.InvasionId.Value)), cancellationToken);
        if (invasion is null)
            return;

        invasion.StartNextWave(difficulty.Value, command.CorrelationId);

        await Repository.SaveAsync(invasion, command.MessageId, cancellationToken);
    }
}
