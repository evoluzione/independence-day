using Evoluzione.IndependenceDay.Earth.Domain.Entities;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Infrastructure.Persistence;

namespace Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;

public class CommissionEarthCommandHandler(IRepository repository, ILoggerFactory loggerFactory)
    : CommandHandlerAsync<CommissionEarth>(repository, loggerFactory)
{
    public override async Task HandleAsync(CommissionEarth command, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = (EarthId)command.AggregateId;
        var existing = await Repository.TryGetByIdAsync<EarthDefense>(id, cancellationToken);
        if (existing is not null)
            return;

        var earth = EarthDefense.Commission(id, command.Rounds, command.Integrity, command.CorrelationId);

        await Repository.SaveAsync(earth, command.MessageId, cancellationToken);
    }
}
