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
    /// <summary>Quanto tempo ha la difesa per abbattere una nave prima che tocchi terra.</summary>
    public int ApproachSeconds { get; set; } = Invasion.ApproachSeconds;

    public int TickMs { get; set; } = 500;

    /// <summary>Dopo quanto si torna a dichiarare scaduto il tempo a una nave ancora aperta.</summary>
    public int RetryMs { get; set; } = 2000;
}

/// <summary>
/// Il cronometro della difesa: quando scade, la nave tocca terra.
/// </summary>
/// <remarks>
/// Sta sulla Terra e non nello Spazio perche' e' la citta' a sapere se la nave e' ancora viva: lo
/// Spazio dovrebbe chiederglielo, e fra due servizi non si chiede niente. Manda un comando come
/// chiunque altro, e l'aggregato decide se ha ancora senso.
/// </remarks>
public class ApproachDeadline(
    IServiceScopeFactory scopeFactory,
    IOptions<ApproachSettings> options,
    ILogger<ApproachDeadline> logger) : BackgroundService
{
    /// <summary>
    /// Quando a ogni nave e' stato dichiarato scaduto il tempo l'ultima volta.
    /// </summary>
    /// <remarks>
    /// A ogni tick sarebbe una tempesta; una volta sola lascerebbe appesa la nave il cui comando ha
    /// perso il confronto di versione sull'aggregato. Quindi si insiste, ma piano.
    /// </remarks>
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

                    // La correlazione e' quella con cui la nave e' stata presa in carico: e' cosi'
                    // che chi la stava intercettando riconosce come proprio l'esito che gli torna.
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
                // Un giro mancato non deve fermare il cronometro: si riprova al tick dopo.
                logger.LogError(ex, "[Earth] Giro del cronometro fallito");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
