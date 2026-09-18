using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Space.Domain.Services;
using Evoluzione.IndependenceDay.Space.ReadModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Evoluzione.IndependenceDay.Space.Facade.BackgroundServices;

public class InvasionSettings
{
    public int LaunchIntervalMs { get; set; } = 1000;

    public int TickMs { get; set; } = 100;
}

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

        var plan = difficulty.Value.Plan();

        if (launched < plan.Count)
        {

            var targets = await scope.ServiceProvider.GetRequiredService<ITargetCityService>().Standing(ct);

            if (targets.Count > 0)
            {
                if (DateTime.UtcNow - _lastLaunch < TimeSpan.FromMilliseconds(settings.LaunchIntervalMs))
                    return;

                await scope.ServiceProvider.GetRequiredService<ISpaceFacade>()
                    .LaunchShip(targets[launched % targets.Count], plan.Ships[launched], progress.Wave, ct);
                _lastLaunch = DateTime.UtcNow;

                return;
            }
        }

        if (await ships.ShipsInFlight(progress.Wave, ct) > 0)
            return;

        logger.LogInformation("[Space] Ondata {Wave} conclusa", progress.Wave);

        await scope.ServiceProvider.GetRequiredService<IServiceBus>().SendAsync(
            new EndInvasion(new InvasionId(Invasion.Id), progress.Wave, Guid.NewGuid(), FleetCommand), ct);
    }
}
