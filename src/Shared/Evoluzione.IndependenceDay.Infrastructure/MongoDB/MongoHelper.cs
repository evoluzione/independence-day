using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Evoluzione.IndependenceDay.Infrastructure.MongoDB;

public static class MongoHelper
{
    /// <summary>
    /// Apre il database e registra il serializzatore dei Guid.
    /// </summary>
    /// <remarks>
    /// La rappresentazione standard va imposta una volta per processo e prima di ogni uso: senza,
    /// il driver scrive i Guid come BinData legacy e una query costruita altrove non li ritrova —
    /// senza errore, solo zero documenti.
    /// </remarks>
    public static IMongoDatabase OpenDatabase(MongoDbSettings settings)
    {
        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        var client = new MongoClient(MongoClientSettings.FromConnectionString(settings.ConnectionString));

        return client.GetDatabase(settings.DatabaseName);
    }
}
