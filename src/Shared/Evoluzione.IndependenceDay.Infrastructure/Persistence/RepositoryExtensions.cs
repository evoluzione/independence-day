using Muflone;
using Muflone.Core;
using Muflone.Persistence;

namespace Evoluzione.IndependenceDay.Infrastructure.Persistence;

public static class RepositoryExtensions
{
    public static async Task<TAggregate?> TryGetByIdAsync<TAggregate>(
    this IRepository repository,
    IDomainId id,
    CancellationToken ct = default) where TAggregate : class, IAggregate
    {
        try
        {
            var aggregate = await repository.GetByIdAsync<TAggregate>(id, ct);

            return aggregate is null || aggregate.Version == 0 ? null : aggregate;
        }
        catch (AggregateNotFoundException)
        {
            return null;
        }
    }
}
