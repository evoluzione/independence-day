using Evoluzione.IndependenceDay.Infrastructure.MongoDB;
using Evoluzione.IndependenceDay.Sagas.Infrastructure.MongoDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Muflone.Saga.Persistence;

namespace Evoluzione.IndependenceDay.Sagas.Infrastructure;

public static class ExtensionsHelper
{
    public static IServiceCollection AddSagasInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        IReadOnlyList<string> businessKeyFields)
    {
        var mongoDbSettings = configuration.GetSection("Sagas:MongoDbSettings").Get<MongoDbSettings>()
                              ?? throw new ArgumentException("Manca la sezione Sagas:MongoDbSettings");

        services.AddKeyedSingleton<IMongoDatabase>("sagas-mongodb", (_, _) =>
        {
            var database = MongoHelper.OpenDatabase(mongoDbSettings);
            EnsureIndexes(database, businessKeyFields).GetAwaiter().GetResult();

            return database;
        });

        services.AddScoped<ISagaRepository, MongoSagaRepository>();
        services.AddScoped<ISagaStateLocator, MongoSagaStateLocator>();

        return services;
    }

    private static Task EnsureIndexes(IMongoDatabase database, IReadOnlyList<string> businessKeyFields)
    {

        var models = businessKeyFields.Select(field => new CreateIndexModel<BsonDocument>(
            Builders<BsonDocument>.IndexKeys.Ascending(MongoSagaCollection.StateField(field)),

            new CreateIndexOptions { Name = $"ix_sagas_state_{field.ToLowerInvariant()}" })).ToList();

        return database.GetCollection<BsonDocument>(MongoSagaCollection.Name).Indexes.CreateManyAsync(models);
    }
}
