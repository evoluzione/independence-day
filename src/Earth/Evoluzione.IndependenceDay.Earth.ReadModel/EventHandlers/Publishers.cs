using Muflone;
using Muflone.Core;
using C = Evoluzione.IndependenceDay.Contracts.Events;

namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

public abstract class EarthPublisher<TEvent>(IEventBus bus, ILoggerFactory loggerFactory)
    : DomainEventHandlerAsync<TEvent>(loggerFactory) where TEvent : DomainEvent
{
    protected abstract IntegrationEvent Translated(TEvent @event);

    protected static Contracts.Ids.EarthId Earth(TEvent @event) =>
        Translate.Earth((Messages.DomainIds.EarthId)@event.AggregateId);

    public override Task HandleAsync(TEvent @event, CancellationToken ct = default) =>
        bus.PublishAsync(Translated(@event), ct);
}

public class ShipDetectedPublisher(IEventBus bus, ILoggerFactory loggers)
    : EarthPublisher<EarthShipDetected>(bus, loggers)
{
    protected override IntegrationEvent Translated(EarthShipDetected e) =>
        new C.ShipDetected(Earth(e), Translate.Ship(e.ShipId), Translate.City(e.CityId), e.CorrelationId());
}

public class FireOpenedPublisher(IEventBus bus, ILoggerFactory loggers)
    : EarthPublisher<EarthFireOpened>(bus, loggers)
{
    protected override IntegrationEvent Translated(EarthFireOpened e) =>
        new C.FireOpened(Earth(e), Translate.City(e.CityId), Translate.Ship(e.ShipId), e.RoundsLeft,
            e.CorrelationId());
}

public class FireCeasedPublisher(IEventBus bus, ILoggerFactory loggers)
    : EarthPublisher<EarthFireCeased>(bus, loggers)
{
    protected override IntegrationEvent Translated(EarthFireCeased e) =>
        new C.FireCeased(Earth(e), Translate.City(e.CityId), Translate.Ship(e.ShipId), e.RoundsLeft,
            e.CorrelationId());
}

public class NoCannonReadyPublisher(IEventBus bus, ILoggerFactory loggers)
    : EarthPublisher<EarthNoCannonReady>(bus, loggers)
{
    protected override IntegrationEvent Translated(EarthNoCannonReady e) =>
        new C.NoCannonReady(Earth(e), Translate.Ship(e.ShipId), e.CorrelationId());
}

public class CannonJammedPublisher(IEventBus bus, ILoggerFactory loggers)
    : EarthPublisher<EarthCannonJammed>(bus, loggers)
{
    protected override IntegrationEvent Translated(EarthCannonJammed e) =>
        new C.CannonJammed(Earth(e), Translate.City(e.CityId), Translate.Ship(e.ShipId), e.CorrelationId());
}

public class CannonRepairedPublisher(IEventBus bus, ILoggerFactory loggers)
    : EarthPublisher<EarthCannonRepaired>(bus, loggers)
{
    protected override IntegrationEvent Translated(EarthCannonRepaired e) =>
        new C.CannonRepaired(Earth(e), Translate.City(e.CityId), Translate.Ship(e.ShipId), e.RoundsLeft,
            e.CorrelationId());
}

public class CannonEmptyPublisher(IEventBus bus, ILoggerFactory loggers)
    : EarthPublisher<EarthCannonEmpty>(bus, loggers)
{
    protected override IntegrationEvent Translated(EarthCannonEmpty e) =>
        new C.CannonEmpty(Earth(e), Translate.City(e.CityId), Translate.Ship(e.ShipId), e.CorrelationId());
}

public class CannonResuppliedPublisher(IEventBus bus, ILoggerFactory loggers)
    : EarthPublisher<EarthCannonResupplied>(bus, loggers)
{
    protected override IntegrationEvent Translated(EarthCannonResupplied e) =>
        new C.CannonResupplied(Earth(e), Translate.City(e.CityId), Translate.Ship(e.ShipId), e.Rounds,
            e.CorrelationId());
}

public class ShipDestroyedPublisher(IEventBus bus, ILoggerFactory loggers)
    : EarthPublisher<EarthShipDestroyed>(bus, loggers)
{
    protected override IntegrationEvent Translated(EarthShipDestroyed e) =>
        new C.ShipDestroyed(Earth(e), Translate.Ship(e.ShipId), Translate.City(e.CityId), e.CorrelationId());
}

public class ShipLandedPublisher(IEventBus bus, ILoggerFactory loggers)
    : EarthPublisher<EarthShipLanded>(bus, loggers)
{
    protected override IntegrationEvent Translated(EarthShipLanded e) =>
        new C.ShipLanded(Earth(e), Translate.Ship(e.ShipId), Translate.City(e.CityId), e.Damage,
            e.IntegrityLeft, e.CorrelationId());
}

public class CityFallenPublisher(IEventBus bus, ILoggerFactory loggers)
    : EarthPublisher<EarthCityFallen>(bus, loggers)
{
    protected override IntegrationEvent Translated(EarthCityFallen e) =>
        new C.CityFallen(Earth(e), Translate.City(e.CityId), Translate.Ship(e.ShipId), e.CorrelationId());
}
