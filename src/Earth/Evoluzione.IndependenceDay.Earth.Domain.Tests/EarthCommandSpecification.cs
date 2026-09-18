using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Messages.DomainIds;
using Evoluzione.IndependenceDay.Earth.Messages.Events;
using Microsoft.Extensions.Logging.Abstractions;
using Muflone.CustomTypes;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

public abstract class EarthCommandSpecification<TCommand> : Muflone.SpecificationTests.CommandSpecification<TCommand>
    where TCommand : Command
{
    protected static readonly Account Coordinator = new("saga", "Ship Interception");
    protected static readonly EarthId Earth = new(Cities.DefenseId);
    protected static readonly Guid CityGuid = Cities.All[0].Id;
    protected static readonly CityId City = new(CityGuid);

    protected static readonly Contracts.Ids.EarthId SharedEarth = new(Cities.DefenseId);

    protected static readonly Contracts.Ids.CityId SharedCity = new(CityGuid);

    protected static Contracts.Ids.ShipId SharedShip(ShipId id) => new(Guid.Parse(id.Value));

    protected static NullLoggerFactory LoggerFactory { get; } = new();

    protected static IEnumerable<DomainEvent> EarthStanding(
    int rounds = Armory.RoundsPerCity,
    int integrity = Ships.FullIntegrity)
    {
        yield return new EarthCommissioned(Earth, rounds, Guid.NewGuid());

        foreach (var city in Cities.All)
            yield return new EarthCityCommissioned(Earth, new CityId(city.Id), city.Name, integrity, rounds,
                Guid.NewGuid());
    }
}
