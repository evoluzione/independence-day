using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.DomainIds;
using Evoluzione.IndependenceDay.Earth.Facade.Messaging;
using Evoluzione.IndependenceDay.Earth.ReadModel;
using Evoluzione.IndependenceDay.Infrastructure.Messaging;

namespace Evoluzione.IndependenceDay.Earth.Facade.IntegrationEventHandlers;

/// <summary>
/// Una nuova ondata.
/// </summary>
/// <remarks>
/// Le citta' si rimettono in linea <b>solo al livello uno</b>, cioe' quando comincia una campagna
/// nuova. Fra un livello e l'altro non si ripristina niente: quello che una saga ha speso resta
/// speso, ed e' esattamente quello che decide fino a che ondata si resiste.
/// <para>
/// Il ripristino passa da un comando per citta', non da una scrittura diretta sul read model: sono
/// aggregati, e "le difese sono state rimesse a posto" e' un fatto che deve stare nel loro stream.
/// </para>
/// </remarks>
public class InvasionStartedIntegrationEventHandler(
    IServiceBus serviceBus,
    IBattleService battle,
    RadioLink radio,
    BattleFeed feed,
    ILoggerFactory loggerFactory) : IntegrationEventHandlerAsync<InvasionStarted>(loggerFactory)
{
    public override async Task HandleAsync(InvasionStarted @event,
        CancellationToken cancellationToken = default)
    {
        await battle.WaveStarted(@event.Wave, @event.Level, @event.Ships, @event.When(), cancellationToken);

        if (@event.Level == 1)
        {
            // Anche il collegamento riparte da capo: altrimenti quali ordini si perdono dipende da
            // quante campagne sono gia' state giocate, e due partite uguali non si somigliano.
            radio.Reset();

            await serviceBus.SendAsync(
                new RecommissionEarth(EarthDefenses.Id, EarthDefenses.Rounds, @event.Wave,
                    EarthDefenses.Integrity, @event.CorrelationId(), EarthDefenses.HighCommand),
                cancellationToken);
        }

        await battle.LogInWave(Guid.Empty, "wave-start",
            $"livello {@event.Level}: {@event.Ships} navi in arrivo",
            Guid.Empty, "saga", @event.Wave, @event.When(), cancellationToken);

        feed.Notify();
    }
}
