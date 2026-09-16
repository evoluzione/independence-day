using Muflone;
using C = Evoluzione.IndependenceDay.Contracts.Events;
using CCityId = Evoluzione.IndependenceDay.Contracts.Ids.CityId;
using CEarthId = Evoluzione.IndependenceDay.Contracts.Ids.EarthId;
using CShipId = Evoluzione.IndependenceDay.Contracts.Ids.ShipId;

namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

/// <summary>
/// Il punto in cui un fatto della Terra diventa un fatto per tutti.
/// </summary>
/// <remarks>
/// Gli eventi di dominio restano dentro il contesto che li ha scritti: quelli che escono sul bus sono
/// una traduzione esplicita, con gli identificativi di <c>Contracts</c>.
/// <para>
/// Non tutti escono. <c>EarthOrderLost</c>, <c>EarthShotFired</c> e <c>EarthShotWasted</c> non hanno
/// una traduzione, e non per dimenticanza: il primo <b>deve</b> restare silenzioso, e gli altri due
/// sono cronaca del tiro che riguarda la pagina, non chi coordina.
/// </para>
/// </remarks>
internal static class Translate
{
    public static CEarthId Earth(Messages.DomainIds.EarthId id) => new(Guid.Parse(id.Value));
    public static CCityId City(Messages.DomainIds.CityId id) => new(Guid.Parse(id.Value));
    public static CShipId Ship(Messages.DomainIds.ShipId id) => new(Guid.Parse(id.Value));
}
