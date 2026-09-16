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
        services.AddScoped<IOperationsRoom, MongoOperationsRoom>();

        return services;
    }

    private static Task EnsureIndexes(IMongoDatabase database, IReadOnlyList<string> businessKeyFields)
    {
        // Si indicizza solo cio' che si interroga davvero: SaveAsync e' una ReplaceOne che gira a ogni
        // passo di ogni saga, e ogni indice in piu' si paga su quella scrittura. _id e' gia' indicizzato.
        var models = businessKeyFields.Select(field => new CreateIndexModel<BsonDocument>(
            Builders<BsonDocument>.IndexKeys.Ascending(MongoSagaCollection.StateField(field)),
            // Il nome e' derivato e non scelto: un indice con un nome diverso da quello gia' creato non
            // sostituisce il vecchio, gli si affianca, e le scritture finiscono per pagarli entrambi.
            new CreateIndexOptions { Name = $"ix_sagas_state_{field.ToLowerInvariant()}" })).ToList();

        return database.GetCollection<BsonDocument>(MongoSagaCollection.Name).Indexes.CreateManyAsync(models);
    }
}
