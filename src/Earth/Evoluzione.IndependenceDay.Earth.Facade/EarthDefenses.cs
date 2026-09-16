using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Messages.DomainIds;
using Muflone.CustomTypes;

namespace Evoluzione.IndependenceDay.Earth.Facade;

/// <summary>Con cosa nasce la difesa, e chi firma i suoi ordini.</summary>
public static class EarthDefenses
{
    public static EarthId Id => new(Cities.DefenseId);

    public const int Rounds = Armory.RoundsPerCity;
    public const int Integrity = Ships.FullIntegrity;

    public static readonly Account HighCommand = new("earth", "Earth High Command");
}
