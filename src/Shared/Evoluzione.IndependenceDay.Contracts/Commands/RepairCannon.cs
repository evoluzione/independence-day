namespace Evoluzione.IndependenceDay.Contracts.Commands;

/// <summary>
/// Rimetti in sesto un cannone inceppato.
/// </summary>
/// <remarks>
/// Un cannone inceppato non spara e non si sblocca da solo: resta fuori uso per il resto della
/// campagna se nessuno se ne occupa. La riparazione costa qualche colpo e <b>non</b> riapre il fuoco:
/// il cannone torna semplicemente disponibile.
/// </remarks>
public sealed class RepairCannon(EarthId aggregateId, CityId cityId, ShipId shipId, Guid correlationId, Account who)
    : DefenseCommand(aggregateId, correlationId, who)
{
    public CityId CityId { get; } = cityId;

    /// <summary>La nave per cui si stava sparando: serve solo a far tornare l'esito a chi l'ha chiesto.</summary>
    public ShipId ShipId { get; } = shipId;
}
