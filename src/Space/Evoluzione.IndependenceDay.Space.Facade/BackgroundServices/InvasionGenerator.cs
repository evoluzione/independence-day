using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Space.Domain.Services;
using Evoluzione.IndependenceDay.Space.ReadModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Evoluzione.IndependenceDay.Space.Facade.BackgroundServices;

public class InvasionSettings
{
    /// <summary>Quanto passa fra una nave e l'altra dentro la stessa ondata.</summary>
    public int LaunchIntervalMs { get; set; } = 1300;

    /// <summary>Ogni quanto il generatore si guarda intorno.</summary>
    public int TickMs { get; set; } = 500;
}

/// <summary>
/// Manda in volo le navi dell'ondata in corso, e la chiude quando non ne resta nessuna.
/// </summary>
/// <remarks>
/// Non decide niente: legge il piano dell'ondata dal read model e lo esegue. E' quello stato — un
/// aggregato, non un campo qui dentro — a sapere a che punto siamo, cosi' un riavvio del servizio
/// ritrova l'ondata dov'era invece di spegnerla.
/// <para>
/// Un'ondata e' finita quando tutte le sue navi sono partite e nessuna e' piu' in avvicinamento.
/// Lo Spazio lo sa da solo: gli esiti gli tornano dalla Terra sul bus e finiscono nelle sue navi,
/// quindi non serve che nessuno gli dica quando ha finito.
/// </para>
/// </remarks>
public class InvasionGenerator(
    IServiceScopeFactory scopeFactory,
    IOptions<InvasionSettings> options,
    IOptions<WaveDifficulty> difficulty,
    ILogger<InvasionGenerator> logger) : BackgroundService
{
    private static readonly Account FleetCommand = new("space", "Ship Command");

    private DateTime _lastLaunch = DateTime.MinValue;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;
        logger.LogInformation("[Space] In attesa dell'ordine di attacco");

        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(settings.TickMs));
        do
        {
            try
            {
                await Tick(settings, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                // Un giro mancato non deve spegnere l'invasione: si riprova al tick dopo.
                logger.LogError(ex, "[Space] Giro del generatore fallito");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task Tick(InvasionSettings settings, CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();

        var progress = await scope.ServiceProvider.GetRequiredService<IInvasionProgressService>()
            .Current(Invasion.Id, ct);
        if (progress is null || !progress.Running)
            return;

        var ships = scope.ServiceProvider.GetRequiredService<IShipsService>();
        var launched = await ships.ShipsLaunched(progress.Wave, ct);

        // Il piano non si porta dietro: si ricalcola dal livello. E' deterministico, quindi un
        // riavvio del servizio ritrova l'ondata esattamente dov'era.
        var plan = difficulty.Value.For(progress.Level);

        if (launched < plan.Count)
        {
            // Solo citta' ancora in piedi: una nave su macerie e' un bersaglio mancato.
            var targets = await scope.ServiceProvider.GetRequiredService<ITargetCityService>().Standing(ct);

            // Niente piu' bersagli: le navi che restano non partiranno mai, quindi l'ondata finisce
            // qui. Aspettarle vorrebbe dire aspettare per sempre, con la partita ferma a schermo.
            if (targets.Count > 0)
            {
                if (DateTime.UtcNow - _lastLaunch < TimeSpan.FromMilliseconds(settings.LaunchIntervalMs))
                    return;

                // A turno, non a caso: con cinque navi e cinque citta' ognuna riceve la sua, e due
                // squadre diverse affrontano la stessa identica invasione.
                await scope.ServiceProvider.GetRequiredService<ISpaceFacade>()
                    .LaunchShip(targets[launched % targets.Count], plan.Ships[launched], progress.Wave, ct);
                _lastLaunch = DateTime.UtcNow;

                return;
            }
        }

        if (await ships.ShipsInFlight(progress.Wave, ct) > 0)
            return;

        logger.LogInformation("[Space] Ondata {Wave} (livello {Level}) conclusa", progress.Wave, progress.Level);

        await scope.ServiceProvider.GetRequiredService<IServiceBus>().SendAsync(
            new EndInvasion(new InvasionId(Invasion.Id), progress.Wave, Guid.NewGuid(), FleetCommand), ct);
    }
}
