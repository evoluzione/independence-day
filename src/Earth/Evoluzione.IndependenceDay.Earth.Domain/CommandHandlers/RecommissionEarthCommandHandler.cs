using Evoluzione.IndependenceDay.Earth.Domain.Entities;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Infrastructure.Persistence;

namespace Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;

public class RecommissionEarthCommandHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerAsync<RecommissionEarth>(repository, loggerFactory)
{
    public override async Task HandleAsync(RecommissionEarth command, CancellationToken cancellationToken = default)
    {
        var earth = await Repository.TryGetByIdAsync<EarthDefense>((EarthId)command.AggregateId, cancellationToken);
        if (earth is null)
            return;

        earth.Recommission(command.Wave, command.Rounds, command.Integrity, command.CorrelationId);

        await Repository.SaveAsync(earth, command.MessageId, cancellationToken);
    }
}
