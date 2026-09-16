using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Earth.Facade.BackgroundServices;
using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Earth.Facade.IntegrationEventHandlers;
using Evoluzione.IndependenceDay.Earth.Facade.Messaging;
using Evoluzione.IndependenceDay.Earth.ReadModel;
using Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;
using Evoluzione.IndependenceDay.Infrastructure.EventStore;
using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using Evoluzione.IndependenceDay.Infrastructure.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Muflone.Eventstore.gRPC;
using Muflone.Eventstore.gRPC.Persistence;

namespace Evoluzione.IndependenceDay.Earth.Facade;

public static class ModuleExtensions
{
    public static IServiceCollection AddEarth(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoDbSettings = configuration.GetSection("Earth:MongoDbSettings").Get<MongoDbSettings>()
                              ?? throw new ArgumentException("Manca la sezione Earth:MongoDbSettings");
        var eventStoreSettings = configuration.GetSection("Earth:EventStore").Get<EventStoreSettings>()
                                 ?? throw new ArgumentException("Manca la sezione Earth:EventStore");
        var rabbitMqSettings = configuration.GetSection("Earth:RabbitMQ").Get<RabbitMqSettings>()
                               ?? throw new ArgumentException("Manca la sezione Earth:RabbitMQ");

        services.AddKeyedSingleton<IMongoDatabase>("earth-mongodb", (_, _) => MongoHelper.OpenDatabase(mongoDbSettings));
        services.AddSingleton<IEventStorePositionRepository>(sp =>
            new EventStorePositionRepository(sp.GetRequiredKeyedService<IMongoDatabase>("earth-mongodb")));

        services.AddMufloneEventStore(eventStoreSettings.ConnectionString);
        services.AddRabbitMq(rabbitMqSettings);

        services.AddSingleton<BattleFeed>();

        // Il collegamento con chi coordina, contatore compreso: uno solo per tutto il servizio.
        services.AddSingleton<RadioLink>();
        services.AddScoped<IBattleService, BattleService>();

        services.Configure<ApproachSettings>(configuration.GetSection("Earth:Approach"));
        services.Configure<FireSettings>(configuration.GetSection("Earth:Fire"));
        services.Configure<HeartbeatSettings>(configuration.GetSection("Earth:Heartbeat"));
        services.AddHostedService<EarthSeeder>();
        services.AddHostedService<ApproachDeadline>();
        services.AddHostedService<FireControl>();
        services.AddHostedService<Heartbeat>();

        services.AddEarthMessageHandlers();

        return services;
    }

    // internal e non private: i test di architettura verificano che ogni handler del contesto sia
    // registrato qui, senza dover aprire Mongo, EventStore e RabbitMQ per scoprirlo.
    internal static IServiceCollection AddEarthMessageHandlers(this IServiceCollection services)
    {
        services.AddCommandHandler<CommissionEarthCommandHandler>();
        services.AddCommandHandler<RecommissionEarthCommandHandler>();
        services.AddCommandHandler<DetectShipCommandHandler>();
        services.AddCommandHandler<PullTriggerCommandHandler>();
        services.AddCommandHandler<LandShipCommandHandler>();

        // I tre ordini che arrivano da fuori passano dalla radio, che ogni tanto ne perde uno. Gli
        // altri no: quelli la Terra li manda a se stessa, e non attraversano niente.
        services.AddScoped<OpenFireCommandHandler>();
        services.AddScoped<CeaseFireCommandHandler>();
        services.AddScoped<RepairCannonCommandHandler>();
        services.AddCommandHandler<OverRadio<OpenFire, OpenFireCommandHandler>>();
        services.AddCommandHandler<OverRadio<CeaseFire, CeaseFireCommandHandler>>();
        services.AddCommandHandler<OverRadio<RepairCannon, RepairCannonCommandHandler>>();

        // Proiezioni: quello che la dashboard legge.
        services.AddDomainEventHandler<EarthCityCommissionedEventHandler>();
        services.AddDomainEventHandler<EarthRecommissionedEventHandler>();
        services.AddDomainEventHandler<EarthShipDetectedEventHandler>();
        services.AddDomainEventHandler<EarthFireOpenedEventHandler>();
        services.AddDomainEventHandler<EarthFireCeasedEventHandler>();
        services.AddDomainEventHandler<EarthNoCannonReadyEventHandler>();
        services.AddDomainEventHandler<EarthShotFiredEventHandler>();
        services.AddDomainEventHandler<EarthShotMissedEventHandler>();
        services.AddDomainEventHandler<EarthShotWastedEventHandler>();
        services.AddDomainEventHandler<EarthCannonJammedEventHandler>();
        services.AddDomainEventHandler<EarthCannonRepairedEventHandler>();
        services.AddDomainEventHandler<EarthCannonStillJammedEventHandler>();
        services.AddDomainEventHandler<EarthCannonEmptyEventHandler>();
        services.AddDomainEventHandler<EarthShipDestroyedEventHandler>();
        services.AddDomainEventHandler<EarthShipLandedEventHandler>();
        services.AddDomainEventHandler<EarthCityFallenEventHandler>();

        // Traduzioni verso il bus: quello che chi coordina e lo Spazio ascoltano. Quello che non
        // compare qui non esce: l'ordine perso, e la cronaca del tiro colpo per colpo.
        services.AddDomainEventHandler<ShipDetectedPublisher>();
        services.AddDomainEventHandler<FireOpenedPublisher>();
        services.AddDomainEventHandler<FireCeasedPublisher>();
        services.AddDomainEventHandler<NoCannonReadyPublisher>();
        services.AddDomainEventHandler<CannonJammedPublisher>();
        services.AddDomainEventHandler<CannonRepairedPublisher>();
        services.AddDomainEventHandler<CannonStillJammedPublisher>();
        services.AddDomainEventHandler<CannonEmptyPublisher>();
        services.AddDomainEventHandler<ShipDestroyedPublisher>();
        services.AddDomainEventHandler<ShipLandedPublisher>();
        services.AddDomainEventHandler<CityFallenPublisher>();

        services.AddIntegrationEventHandler<InvasionStartedIntegrationEventHandler>();
        services.AddIntegrationEventHandler<InvasionEndedIntegrationEventHandler>();
        services.AddIntegrationEventHandler<AlienShipDetectedIntegrationEventHandler>();

        return services;
    }
}
