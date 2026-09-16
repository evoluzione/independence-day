using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using Evoluzione.IndependenceDay.Space.ReadModel.Documents;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace Evoluzione.IndependenceDay.Space.ReadModel;

public sealed class TargetCityService([FromKeyedServices("space-mongodb")] IMongoDatabase database)
    : ProjectionPersister<TargetCity>(database), ITargetCityService
{
    public async Task<IReadOnlyList<Guid>> Standing(CancellationToken ct = default)
    {
        var fallen = await Collection.Find(x => !x.Standing).Project(x => x.Id).ToListAsync(ct);

        // Le citta' le conosce la mappa condivisa: qui si tiene solo chi e' caduta, cosi' una citta'
        // mai colpita non ha bisogno di un documento per essere un bersaglio valido.
        return Cities.All.Select(c => c.Id).Where(id => !fallen.Contains(id)).ToList();
    }

    /// <remarks>
    /// Segna solo se la notizia e' piu' recente dell'ultimo azzeramento.
    /// <para>
    /// L'azzeramento nasce da un evento dello Spazio, la caduta da un evento della Terra: due
    /// sottoscrizioni diverse, nessun ordine garantito fra loro. Una citta' caduta in fondo alla
    /// campagna precedente puo' arrivare qui <b>dopo</b> che la nuova e' cominciata, e senza questo
    /// confronto resterebbe fuori dai bersagli per tutta la campagna nuova — fino a non lasciarne
    /// nessuno.
    /// </para>
    /// </remarks>
    public Task MarkFallen(Guid cityId, DateTime at, CancellationToken ct = default) =>
        Collection.UpdateOneAsync(
            Builders<TargetCity>.Filter.And(
                Builders<TargetCity>.Filter.Eq(x => x.Id, cityId),
                Builders<TargetCity>.Filter.Lt(x => x.UpdatedAt, at)),
            Builders<TargetCity>.Update.Set(x => x.Standing, false).Set(x => x.UpdatedAt, at),
            cancellationToken: ct);

    /// <remarks>
    /// Scrive tutte e cinque le citta' invece di aggiornare quelle che esistono: cosi' dopo un
    /// azzeramento il documento c'e' sempre, e <see cref="MarkFallen"/> puo' limitarsi a confrontare
    /// le date senza doverlo creare.
    /// </remarks>
    public Task ResetAll(DateTime at, CancellationToken ct = default) =>
        Collection.BulkWriteAsync(
            Cities.All.Select(city => new ReplaceOneModel<TargetCity>(
                Builders<TargetCity>.Filter.Eq(x => x.Id, city.Id),
                new TargetCity { Id = city.Id, Standing = true, UpdatedAt = at }) { IsUpsert = true }),
            cancellationToken: ct);
}
