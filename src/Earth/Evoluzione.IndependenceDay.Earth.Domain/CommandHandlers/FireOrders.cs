using Evoluzione.IndependenceDay.Earth.Domain.Entities;
using Evoluzione.IndependenceDay.Infrastructure.Persistence;

namespace Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;

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

        await repository.SaveAsync(earth, commitId, ct);
    }

    public static CityId City(Contracts.Ids.CityId id) => new(Guid.Parse(id.Value));

    public static ShipId Ship(Contracts.Ids.ShipId id) => new(Guid.Parse(id.Value));
}
