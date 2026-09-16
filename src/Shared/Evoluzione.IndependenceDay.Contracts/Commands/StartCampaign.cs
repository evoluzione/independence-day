namespace Evoluzione.IndependenceDay.Contracts.Commands;

/// <summary>
/// Comincia una campagna nuova: si riparte dal livello uno con le difese rimesse a nuovo.
/// </summary>
/// <remarks>
/// Nasce nella sala operativa, che sta sulla Terra, e arriva allo Spazio sul bus come ogni altro
/// messaggio: e' un comando di simulazione, non un atto di dominio della Terra.
/// </remarks>
public sealed class StartCampaign(InvasionId aggregateId, Guid correlationId, Account who)
    : DefenseCommand(aggregateId, correlationId, who)
{
    public InvasionId InvasionId { get; } = aggregateId;
}
