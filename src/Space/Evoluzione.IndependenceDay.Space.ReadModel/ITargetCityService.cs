namespace Evoluzione.IndependenceDay.Space.ReadModel;

public interface ITargetCityService
{
    /// <summary>Le citta' su cui ha ancora senso mandare una nave.</summary>
    Task<IReadOnlyList<Guid>> Standing(CancellationToken ct = default);

    Task MarkFallen(Guid cityId, DateTime at, CancellationToken ct = default);

    /// <summary>Campagna nuova: tornano tutte in piedi.</summary>
    Task ResetAll(DateTime at, CancellationToken ct = default);
}
