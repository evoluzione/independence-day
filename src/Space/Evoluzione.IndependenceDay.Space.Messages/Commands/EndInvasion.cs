namespace Evoluzione.IndependenceDay.Space.Messages.Commands;

/// <summary>
/// Chiude l'ondata: la stiva della nave madre e' vuota.
/// </summary>
/// <remarks>
/// Interno allo Spazio. Porta il numero dell'ondata perche' un comando in ritardo non deve chiudere
/// quella dopo: l'aggregato lo confronta con l'ondata in corso e, se non combacia, esce in silenzio.
/// </remarks>
public sealed class EndInvasion(InvasionId aggregateId, int wave, Guid correlationId, Account who)
    : SpaceCommand(aggregateId, correlationId, who)
{
    public InvasionId InvasionId { get; } = aggregateId;
    public int Wave { get; } = wave;
}
