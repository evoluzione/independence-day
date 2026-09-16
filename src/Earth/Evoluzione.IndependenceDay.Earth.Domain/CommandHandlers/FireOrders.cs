using Evoluzione.IndependenceDay.Earth.Domain.Entities;
using Evoluzione.IndependenceDay.Infrastructure.Persistence;

namespace Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;

/// <summary>
/// Il montaggio comune dei tre ordini che arrivano da fuori.
/// </summary>
/// <remarks>
/// Apertura, cessate il fuoco e riparazione sono comandi di <c>Contracts</c> e non della Terra: sono
/// il contratto con chi coordina, che non conosce i tipi interni di questo servizio. Arrivano con
/// gli identificativi condivisi, e qui vengono tradotti in quelli dell'aggregato.
/// </remarks>
internal static class FireOrders
{
    public static async Task On(
        IRepository repository,
        Contracts.Ids.EarthId earthId,
        Action<EarthDefense> order,
        Guid commitId,
        CancellationToken ct)
    {
        var earth = await repository.TryGetByIdAsync<EarthDefense>(new EarthId(Guid.Parse(earthId.Value)), ct);
        if (earth is null)
            return;

        order(earth);

        // Il commitId e' l'identita' dell'append: una riconsegna dello stesso ordine non impegna un
        // secondo cannone, perche' il secondo append viene scartato dall'event store.
        await repository.SaveAsync(earth, commitId, ct);
    }

    public static CityId City(Contracts.Ids.CityId id) => new(Guid.Parse(id.Value));

    public static ShipId Ship(Contracts.Ids.ShipId id) => new(Guid.Parse(id.Value));
}
