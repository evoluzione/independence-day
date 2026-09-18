using Muflone;
using C = Evoluzione.IndependenceDay.Contracts.Events;
using CCityId = Evoluzione.IndependenceDay.Contracts.Ids.CityId;
using CEarthId = Evoluzione.IndependenceDay.Contracts.Ids.EarthId;
using CShipId = Evoluzione.IndependenceDay.Contracts.Ids.ShipId;

namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

internal static class Translate
{
    public static CEarthId Earth(Messages.DomainIds.EarthId id) => new(Guid.Parse(id.Value));
    public static CCityId City(Messages.DomainIds.CityId id) => new(Guid.Parse(id.Value));
    public static CShipId Ship(Messages.DomainIds.ShipId id) => new(Guid.Parse(id.Value));
}
