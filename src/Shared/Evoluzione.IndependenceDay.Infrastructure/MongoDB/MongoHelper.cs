using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Evoluzione.IndependenceDay.Infrastructure.MongoDB;

public static class MongoHelper
{
    public static IMongoDatabase OpenDatabase(MongoDbSettings settings)
    {
        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        var client = new MongoClient(MongoClientSettings.FromConnectionString(settings.ConnectionString));

        return client.GetDatabase(settings.DatabaseName);
    }
}
