using Evoluzione.IndependenceDay.Space.ReadModel.Documents;

namespace Evoluzione.IndependenceDay.Space.ReadModel;

public interface IInvasionProgressService
{
    Task WaveStarted(Guid invasionId, int wave, int level, int ships, DateTime at,
        long revision, CancellationToken ct = default);

    Task WaveEnded(Guid invasionId, DateTime at, long revision, CancellationToken ct = default);

    Task<InvasionProgress?> Current(Guid invasionId, CancellationToken ct = default);
}
