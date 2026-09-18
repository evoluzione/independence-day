using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.DomainIds;
using Evoluzione.IndependenceDay.Earth.ReadModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Evoluzione.IndependenceDay.Earth.Facade.BackgroundServices;

public class FireSettings
{
    public int TickMs { get; set; } = 100;

    public int ReloadMs { get; set; } = Armory.ReloadMs;
}

public class FireControl(
    IServiceScopeFactory scopeFactory,
    IOptions<FireSettings> options,
    ILogger<FireControl> logger) : BackgroundService
{
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

                var ready = DateTime.UtcNow.AddMilliseconds(-settings.ReloadMs);
                var ships = (await battle.OpenShips(stoppingToken)).ToDictionary(s => s.Id);

                foreach (var cannon in await battle.FiringCannons(stoppingToken))
                {
                    if (cannon.LastShotAt > ready)
                        continue;

                    var correlationId = ships.TryGetValue(cannon.Target, out var ship)
                        ? ship.CorrelationId
                        : Guid.NewGuid();

                    await serviceBus.SendAsync(
                        new PullTrigger(EarthDefenses.Id, new CityId(cannon.Id), correlationId,
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

                logger.LogError(ex, "[Earth] Giro della centrale di tiro fallito");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
