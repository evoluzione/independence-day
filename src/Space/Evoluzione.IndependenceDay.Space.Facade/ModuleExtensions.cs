using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Infrastructure.EventStore;
using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using Evoluzione.IndependenceDay.Infrastructure.RabbitMQ;
using Evoluzione.IndependenceDay.Space.Domain.CommandHandlers;
using Evoluzione.IndependenceDay.Space.Domain.Services;
using Evoluzione.IndependenceDay.Space.Facade.BackgroundServices;
using Evoluzione.IndependenceDay.Space.Facade.IntegrationEventHandlers;
using Evoluzione.IndependenceDay.Space.ReadModel;
using Evoluzione.IndependenceDay.Space.ReadModel.EventHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Muflone.Eventstore.gRPC;
using Muflone.Eventstore.gRPC.Persistence;

namespace Evoluzione.IndependenceDay.Space.Facade;

public static class ModuleExtensions
{
    public static IServiceCollection AddSpace(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoDbSettings = configuration.GetSection("Space:MongoDbSettings").Get<MongoDbSettings>()
                              ?? throw new ArgumentException("Manca la sezione Space:MongoDbSettings");
        var eventStoreSettings = configuration.GetSection("Space:EventStore").Get<EventStoreSettings>()
                                 ?? throw new ArgumentException("Manca la sezione Space:EventStore");
        var rabbitMqSettings = configuration.GetSection("Space:RabbitMQ").Get<RabbitMqSettings>()
                               ?? throw new ArgumentException("Manca la sezione Space:RabbitMQ");

        services.AddKeyedSingleton<IMongoDatabase>("space-mongodb", (_, _) => MongoHelper.OpenDatabase(mongoDbSettings));
        services.AddSingleton<IEventStorePositionRepository>(sp =>
            new EventStorePositionRepository(sp.GetRequiredKeyedService<IMongoDatabase>("space-mongodb")));

        services.AddMufloneEventStore(eventStoreSettings.ConnectionString);
        services.AddRabbitMq(rabbitMqSettings);

        services.AddScoped<ISpaceFacade, SpaceFacade>();
        services.AddScoped<IShipsService, ShipsService>();
        services.AddScoped<IInvasionProgressService, InvasionProgressService>();
        services.AddScoped<ITargetCityService, TargetCityService>();

        services.Configure<InvasionSettings>(configuration.GetSection("Space:Invasion"));
        services.Configure<WaveDifficulty>(configuration.GetSection("Space:Difficulty"));
        services.AddHostedService<InvasionGenerator>();

        services.AddSpaceMessageHandlers();

        return services;
    }

    // internal e non private: i test di architettura verificano che ogni handler del contesto sia
    // registrato qui, senza dover aprire Mongo, EventStore e RabbitMQ per scoprirlo.
    internal static IServiceCollection AddSpaceMessageHandlers(this IServiceCollection services)
    {
        services.AddCommandHandler<StartCampaignCommandHandler>();
        services.AddCommandHandler<StartNextWaveCommandHandler>();
        services.AddCommandHandler<EndInvasionCommandHandler>();
        services.AddCommandHandler<LaunchAlienShipCommandHandler>();
        services.AddCommandHandler<DestroyAlienShipCommandHandler>();
        services.AddCommandHandler<LandAlienShipCommandHandler>();

        services.AddDomainEventHandler<InvasionWaveStartedEventHandler>();
        services.AddDomainEventHandler<CitiesResetOnNewCampaignEventHandler>();
        services.AddDomainEventHandler<InvasionWaveEndedEventHandler>();
        services.AddDomainEventHandler<InvasionStartedPublisher>();
        services.AddDomainEventHandler<InvasionEndedPublisher>();
        services.AddDomainEventHandler<AlienShipLaunchedEventHandler>();
        services.AddDomainEventHandler<AlienShipDetectedPublisher>();
        services.AddDomainEventHandler<AlienShipDestroyedEventHandler>();
        services.AddDomainEventHandler<AlienShipLandedEventHandler>();

        services.AddIntegrationEventHandler<CityFallenIntegrationEventHandler>();
        services.AddIntegrationEventHandler<ShipDestroyedIntegrationEventHandler>();
        services.AddIntegrationEventHandler<ShipLandedIntegrationEventHandler>();

        return services;
    }
}
