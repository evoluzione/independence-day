using Evoluzione.IndependenceDay.Space.Domain.Services;
using Muflone.Core;

namespace Evoluzione.IndependenceDay.Space.Domain.Entities;

public enum InvasionStatus
{
    Idle = 0,

    Running = 1
}

public class Invasion : AggregateRoot
{
    protected Invasion()
    {
    }

    protected Invasion(InvasionId id, WavePlan plan, Guid correlationId)
    {
        ArgumentNullException.ThrowIfNull(id);

        if (correlationId == Guid.Empty)
            throw new ArgumentException("CorrelationId cannot be empty.", nameof(correlationId));

        RaiseEvent(new InvasionWaveStarted(id, 1, 1, plan.Count, correlationId));
    }

    public int Wave { get; private set; }
    public int Level { get; private set; }
    public InvasionStatus Status { get; private set; }

    public static Invasion Begin(InvasionId id, WavePlan plan, Guid correlationId) => new(id, plan, correlationId);

    public void StartCampaign(WaveDifficulty difficulty, Guid correlationId)
    {
        if (Status == InvasionStatus.Running)
            RaiseEvent(new InvasionWaveEnded((InvasionId)Id, Wave, Level, correlationId));

        var plan = difficulty.Plan();

        RaiseEvent(new InvasionWaveStarted((InvasionId)Id, Wave + 1, 1, plan.Count, correlationId));
    }

    public void EndWave(int wave, Guid correlationId)
    {

        if (Status != InvasionStatus.Running || wave != Wave)
            return;

        RaiseEvent(new InvasionWaveEnded((InvasionId)Id, Wave, Level, correlationId));
    }

    public void Apply(InvasionWaveStarted @event)
    {
        Id = @event.AggregateId;
        Wave = @event.Wave;
        Level = @event.Level;
        Status = InvasionStatus.Running;
    }

    public void Apply(InvasionWaveEnded @event) => Status = InvasionStatus.Idle;
}
