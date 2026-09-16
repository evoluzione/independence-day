using Evoluzione.IndependenceDay.Space.Domain.Services;
using Muflone.Core;

namespace Evoluzione.IndependenceDay.Space.Domain.Entities;

public enum InvasionStatus
{
    /// <summary>Nessuna ondata in volo: o non e' mai cominciata, o l'ultima e' finita.</summary>
    Idle = 0,

    Running = 1
}

/// <summary>
/// L'andamento dell'invasione: a che ondata siamo, a che livello, e se e' in corso.
/// </summary>
/// <remarks>
/// Due contatori, e servono entrambi. <b>Wave</b> e' un progressivo di sempre che non si azzera
/// nemmeno fra una campagna e l'altra: e' la chiave con cui read model e diario si filtrano, e due
/// campagne diverse non devono ritrovarsi a condividere lo stesso numero. <b>Level</b> e' la
/// posizione nella campagna in corso: e' la difficolta', ed e' il punteggio.
/// <para>
/// Cominciare una campagna azzera il livello, passare all'ondata dopo lo incrementa — e non tocca le
/// difese, che restano come le ha lasciate l'ondata precedente. E' quella la regola del gioco.
/// </para>
/// </remarks>
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

    /// <summary>Campagna nuova: si torna al livello uno, e l'ondata in corso viene chiusa.</summary>
    public void StartCampaign(WaveDifficulty difficulty, Guid correlationId)
    {
        if (Status == InvasionStatus.Running)
            RaiseEvent(new InvasionWaveEnded((InvasionId)Id, Wave, Level, correlationId));

        var plan = difficulty.For(1);

        RaiseEvent(new InvasionWaveStarted((InvasionId)Id, Wave + 1, 1, plan.Count, correlationId));
    }

    /// <summary>
    /// Ondata successiva, un livello piu' su. Non fa niente se quella in corso non e' ancora finita:
    /// due ondate insieme non sono un livello piu' difficile, sono un altro gioco.
    /// </summary>
    public void StartNextWave(WaveDifficulty difficulty, Guid correlationId)
    {
        if (Status == InvasionStatus.Running)
            return;

        // Oltre l'ultimo livello non c'e' niente: la campagna e' gia' vinta.
        if (Level >= Contracts.World.Invasion.LastLevel)
            return;

        var plan = difficulty.For(Level + 1);

        RaiseEvent(new InvasionWaveStarted((InvasionId)Id, Wave + 1, Level + 1, plan.Count, correlationId));
    }

    public void EndWave(int wave, Guid correlationId)
    {
        // Un comando di chiusura in ritardo non deve chiudere l'ondata dopo.
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
