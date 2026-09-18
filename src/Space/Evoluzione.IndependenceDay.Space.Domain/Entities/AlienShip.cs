using Evoluzione.IndependenceDay.Contracts.World;
using Muflone.Core;

namespace Evoluzione.IndependenceDay.Space.Domain.Entities;

public class AlienShip : AggregateRoot
{
    protected AlienShip()
    {
    }

    protected AlienShip(ShipId id, CityId targetCity, MotherShipId motherShip, ShipClass shipClass, int wave,
        Guid correlationId)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(targetCity);
        ArgumentNullException.ThrowIfNull(motherShip);

        if (correlationId == Guid.Empty)
            throw new ArgumentException("CorrelationId cannot be empty.", nameof(correlationId));

        RaiseEvent(new AlienShipLaunched(id, targetCity, motherShip, shipClass, wave, correlationId));
    }

    public CityId TargetCity { get; private set; } = null!;
    public MotherShipId MotherShip { get; private set; } = null!;
    public ShipClass Class { get; private set; }
    public int Wave { get; private set; }
    public ShipStatus Status { get; private set; }

    public static AlienShip Launch(
        ShipId id,
        CityId targetCity,
        MotherShipId motherShip,
        ShipClass shipClass,
        int wave,
        Guid correlationId) => new(id, targetCity, motherShip, shipClass, wave, correlationId);

    public void Destroy(string cause, Guid correlationId)
    {
        if (Status != ShipStatus.Approaching)
            return;

        RaiseEvent(new AlienShipDestroyed((ShipId)Id, cause, correlationId));
    }

    public void Land(CityId cityId, Guid correlationId)
    {
        if (Status != ShipStatus.Approaching)
            return;

        if (!Equals(cityId.Value, TargetCity.Value))
            return;

        RaiseEvent(new AlienShipLanded((ShipId)Id, cityId, correlationId));
    }

    public void Apply(AlienShipLaunched @event)
    {
        Id = @event.AggregateId;
        TargetCity = @event.TargetCity;
        MotherShip = @event.MotherShip;
        Class = @event.ShipClass;
        Wave = @event.Wave;
        Status = ShipStatus.Approaching;
    }

    public void Apply(AlienShipDestroyed @event) => Status = ShipStatus.Destroyed;

    public void Apply(AlienShipLanded @event) => Status = ShipStatus.Landed;
}
