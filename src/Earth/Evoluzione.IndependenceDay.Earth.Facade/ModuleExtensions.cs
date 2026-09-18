using Evoluzione.IndependenceDay.Earth.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Earth.Facade.BackgroundServices;
using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Earth.Facade.IntegrationEventHandlers;
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

        services.AddScoped<IBattleService, BattleService>();

        services.Configure<ApproachSettings>(configuration.GetSection("Earth:Approach"));
        services.Configure<FireSettings>(configuration.GetSection("Earth:Fire"));
        services.Configure<HeartbeatSettings>(configuration.GetSection("Earth:Heartbeat"));
        services.Configure<ResupplySettings>(configuration.GetSection("Earth:Resupply"));
        services.AddHostedService<EarthSeeder>();
        services.AddHostedService<ApproachDeadline>();
        services.AddHostedService<FireControl>();
        services.AddHostedService<Heartbeat>();
        services.AddHostedService<SupplyConvoy>();

        services.AddEarthMessageHandlers();

        return services;
    }

    internal static IServiceCollection AddEarthMessageHandlers(this IServiceCollection services)
    {
        services.AddCommandHandler<CommissionEarthCommandHandler>();
        services.AddCommandHandler<RecommissionEarthCommandHandler>();
        services.AddCommandHandler<DetectShipCommandHandler>();
        services.AddCommandHandler<PullTriggerCommandHandler>();
        services.AddCommandHandler<LandShipCommandHandler>();

        services.AddCommandHandler<OpenFireCommandHandler>();
        services.AddCommandHandler<CeaseFireCommandHandler>();
        services.AddCommandHandler<RepairCannonCommandHandler>();
        services.AddCommandHandler<RequestResupplyCommandHandler>();
        services.AddCommandHandler<DeliverSuppliesCommandHandler>();

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
        services.AddDomainEventHandler<EarthCannonEmptyEventHandler>();
        services.AddDomainEventHandler<EarthResupplyDispatchedEventHandler>();
        services.AddDomainEventHandler<EarthCannonResuppliedEventHandler>();
        services.AddDomainEventHandler<EarthShipDestroyedEventHandler>();
        services.AddDomainEventHandler<EarthShipLandedEventHandler>();
        services.AddDomainEventHandler<EarthCityFallenEventHandler>();

        services.AddDomainEventHandler<ShipDetectedPublisher>();
        services.AddDomainEventHandler<FireOpenedPublisher>();
        services.AddDomainEventHandler<FireCeasedPublisher>();
        services.AddDomainEventHandler<NoCannonReadyPublisher>();
        services.AddDomainEventHandler<CannonJammedPublisher>();
        services.AddDomainEventHandler<CannonRepairedPublisher>();
        services.AddDomainEventHandler<CannonEmptyPublisher>();
        services.AddDomainEventHandler<CannonResuppliedPublisher>();
        services.AddDomainEventHandler<ShipDestroyedPublisher>();
        services.AddDomainEventHandler<ShipLandedPublisher>();
        services.AddDomainEventHandler<CityFallenPublisher>();

        services.AddIntegrationEventHandler<InvasionStartedIntegrationEventHandler>();
        services.AddIntegrationEventHandler<InvasionEndedIntegrationEventHandler>();
        services.AddIntegrationEventHandler<AlienShipDetectedIntegrationEventHandler>();

        return services;
    }
}
