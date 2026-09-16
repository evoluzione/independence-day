using Muflone;
using Muflone.Core;
using Muflone.Persistence;

namespace Evoluzione.IndependenceDay.Infrastructure.Persistence;

public static class RepositoryExtensions
{
    /// <summary>
    /// Carica un aggregato, oppure <c>null</c> se non e' mai stato scritto.
    /// </summary>
    /// <remarks>
    /// <c>GetByIdAsync</c> solleva <see cref="AggregateNotFoundException" /> quando lo stream non
    /// esiste. Per un comando che crea (esiste gia'?) o per uno che arriva prima della creazione,
    /// "non c'e'" e' una risposta, non un guasto: tradurla in <c>null</c> evita che ogni handler si
    /// porti dietro il proprio try/catch, e che qualcuno se lo dimentichi.
    /// </remarks>
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
