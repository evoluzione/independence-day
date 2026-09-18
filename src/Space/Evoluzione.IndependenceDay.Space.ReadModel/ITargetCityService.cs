namespace Evoluzione.IndependenceDay.Space.ReadModel;

public interface ITargetCityService
{
    Task<IReadOnlyList<Guid>> Standing(CancellationToken ct = default);

    Task MarkFallen(Guid cityId, DateTime at, CancellationToken ct = default);

    Task ResetAll(DateTime at, CancellationToken ct = default);
}
