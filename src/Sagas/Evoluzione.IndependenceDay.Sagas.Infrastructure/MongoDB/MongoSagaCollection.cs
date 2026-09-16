namespace Evoluzione.IndependenceDay.Sagas.Infrastructure.MongoDB;

/// <summary>
/// I nomi con cui si accede alla collection delle saghe.
/// </summary>
/// <remarks>
/// Stanno in un posto solo perche' li usano tre punti che devono essere d'accordo: chi scrive, chi
/// cerca per chiave di business e chi costruisce gli indici. Un indice su un nome di campo diverso da
/// quello interrogato non da' errore: semplicemente non viene usato.
/// </remarks>
internal static class MongoSagaCollection
{
    public const string Name = "sagas";
    public const string CorrelationIdField = "_id";
    public const string StateTypeField = "stateType";
    public const string StatusField = "state.Status";

    public static string StateField(string propertyName) => $"state.{propertyName}";
}
