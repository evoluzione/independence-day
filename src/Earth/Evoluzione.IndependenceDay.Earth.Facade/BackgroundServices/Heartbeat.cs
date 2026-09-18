using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Contracts.Ids;
using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.ReadModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Muflone;

namespace Evoluzione.IndependenceDay.Earth.Facade.BackgroundServices;

public class HeartbeatSettings
{
    public int TickMs { get; set; } = 500;
}

public class Heartbeat(
    IServiceScopeFactory scopeFactory,
    IOptions<HeartbeatSettings> options,
    ILogger<Heartbeat> logger) : BackgroundService
{
    private static readonly EarthId Earth = new(Cities.DefenseId);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;
        var approachMs = Invasion.ApproachSeconds * 1000;

        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(settings.TickMs));
        do
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var battle = scope.ServiceProvider.GetRequiredService<IBattleService>();
                var bus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                var now = DateTime.UtcNow;
                var cannons = await battle.EngagedCannons(stoppingToken);
                var inFlight = await battle.OpenShips(stoppingToken);

                foreach (var ship in inFlight)
                    await bus.PublishAsync(
                        new ShipApproaching(Earth, new ShipId(ship.Id), new CityId(ship.CityId),
                            Math.Max(0, approachMs - (int)(now - ship.DetectedAt).TotalMilliseconds),
                            cannons.Count(c => c.Target == ship.Id),
                            ship.CorrelationId),
                        stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "[Earth] Battito fallito");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
