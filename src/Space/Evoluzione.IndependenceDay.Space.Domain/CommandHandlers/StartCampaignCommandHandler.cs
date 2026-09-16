using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Infrastructure.Persistence;
using Evoluzione.IndependenceDay.Space.Domain.Entities;
using Evoluzione.IndependenceDay.Space.Domain.Services;
using Microsoft.Extensions.Options;

namespace Evoluzione.IndependenceDay.Space.Domain.CommandHandlers;

public class StartCampaignCommandHandler(
    IRepository repository,


    IOptions<WaveDifficulty> difficulty,
    ILoggerFactory loggerFactory) : CommandHandlerAsync<StartCampaign>(repository, loggerFactory)
{
    public override async Task HandleAsync(StartCampaign command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = new InvasionId(Guid.Parse(command.InvasionId.Value));
        var invasion = await Repository.TryGetByIdAsync<Invasion>(id, cancellationToken);

        if (invasion is null)
            invasion = Invasion.Begin(id, difficulty.Value.For(1), command.CorrelationId);
        else
            invasion.StartCampaign(difficulty.Value, command.CorrelationId);

        await Repository.SaveAsync(invasion, command.MessageId, cancellationToken);
    }
}
