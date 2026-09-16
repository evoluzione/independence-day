using Evoluzione.IndependenceDay.Contracts.World;

namespace Evoluzione.IndependenceDay.Space.Facade;

public sealed class SpaceFacade(IServiceBus serviceBus) : ISpaceFacade
{
    private static readonly Account Invaders = new("space", "Ship Command");

    public async Task<Guid> LaunchShip(Guid targetCityId, ShipClass shipClass, int wave, CancellationToken ct = default)
    {
        var shipId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        await serviceBus.SendAsync(
            new LaunchAlienShip(
                new ShipId(shipId),
                new CityId(targetCityId),
                new MotherShipId(Invasion.MotherShipId),
                shipClass,
                wave,
                correlationId,
                Invaders),
            ct);

        return shipId;
    }
}
