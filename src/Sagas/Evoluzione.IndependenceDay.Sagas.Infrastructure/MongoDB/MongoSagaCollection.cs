namespace Evoluzione.IndependenceDay.Sagas.Infrastructure.MongoDB;

internal static class MongoSagaCollection
{
    public const string Name = "sagas";
    public const string CorrelationIdField = "_id";
    public const string StateTypeField = "stateType";
    public const string StatusField = "state.Status";

    public static string StateField(string propertyName) => $"state.{propertyName}";
}
