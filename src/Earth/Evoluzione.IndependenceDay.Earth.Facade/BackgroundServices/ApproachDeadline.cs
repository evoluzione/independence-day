using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.DomainIds;
using Evoluzione.IndependenceDay.Earth.ReadModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Evoluzione.IndependenceDay.Earth.Facade.BackgroundServices;

public class ApproachSettings
{
    public int ApproachSeconds { get; set; } = Invasion.ApproachSeconds;

    public int TickMs { get; set; } = 100;

    public int RetryMs { get; set; } = 2000;
}

public class ApproachDeadline(
    IServiceScopeFactory scopeFactory,
    IOptions<ApproachSettings> options,
    ILogger<ApproachDeadline> logger) : BackgroundService
{
    private readonly Dictionary<Guid, DateTime> _called = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;

        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(settings.TickMs));
        do
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var battle = scope.ServiceProvider.GetRequiredService<IBattleService>();
                var serviceBus = scope.ServiceProvider.GetRequiredService<IServiceBus>();
                var deadline = DateTime.UtcNow.AddSeconds(-settings.ApproachSeconds);

                var open = await battle.OpenShips(stoppingToken);
                var incoming = open.Select(s => s.Id).ToHashSet();
                foreach (var id in _called.Keys.Where(id => !incoming.Contains(id)).ToList())
                    _called.Remove(id);

                var now = DateTime.UtcNow;

                foreach (var ship in open)
                {
                    if (ship.DetectedAt > deadline)
                        continue;

                    if (_called.TryGetValue(ship.Id, out var lastCall) &&
                        now - lastCall < TimeSpan.FromMilliseconds(settings.RetryMs))
                        continue;

                    _called[ship.Id] = now;

                    await serviceBus.SendAsync(
                        new LandShip(EarthDefenses.Id, new ShipId(ship.Id), ship.CorrelationId,
                            EarthDefenses.HighCommand),
                        stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "[Earth] Giro del cronometro fallito");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
