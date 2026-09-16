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

        // La difesa si mette in piedi a ogni avvio. Se c'e' gia', il suo stream esiste e non va
        // riscritto: rifarla azzererebbe la partita in corso.
        var id = (EarthId)command.AggregateId;
        var existing = await Repository.TryGetByIdAsync<EarthDefense>(id, cancellationToken);
        if (existing is not null)
            return;

        var earth = EarthDefense.Commission(id, command.Rounds, command.Integrity, command.CorrelationId);

        await Repository.SaveAsync(earth, command.MessageId, cancellationToken);
    }
}
