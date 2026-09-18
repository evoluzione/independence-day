using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Evoluzione.IndependenceDay.Earth.Messages.DomainIds;
using Evoluzione.IndependenceDay.Earth.ReadModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Evoluzione.IndependenceDay.Earth.Facade.BackgroundServices;

public class ResupplySettings
{
    public int ResupplySeconds { get; set; } = Armory.ResupplySeconds;

    public int TickMs { get; set; } = 100;

    public int RetryMs { get; set; } = 2000;
}

public class SupplyConvoy(
    IServiceScopeFactory scopeFactory,
    IOptions<ResupplySettings> options,
    ILogger<SupplyConvoy> logger) : BackgroundService
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
                var deadline = DateTime.UtcNow.AddSeconds(-settings.ResupplySeconds);

                var waiting = await battle.ResupplyingCannons(stoppingToken);
                var cities = waiting.Select(c => c.Id).ToHashSet();
                foreach (var id in _called.Keys.Where(id => !cities.Contains(id)).ToList())
                    _called.Remove(id);

                var now = DateTime.UtcNow;

                foreach (var city in waiting)
                {
                    if (city.ResupplyAt > deadline)
                        continue;

                    if (_called.TryGetValue(city.Id, out var lastCall) &&
                        now - lastCall < TimeSpan.FromMilliseconds(settings.RetryMs))
                        continue;

                    var ship = await battle.Ship(city.ResupplyFor, stoppingToken);
                    if (ship is null)
                        continue;

                    _called[city.Id] = now;

                    await serviceBus.SendAsync(
                        new DeliverSupplies(EarthDefenses.Id, new CityId(city.Id), new ShipId(city.ResupplyFor),
                            ship.CorrelationId, EarthDefenses.HighCommand),
                        stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {

                logger.LogError(ex, "[Earth] Giro del convoglio fallito");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
