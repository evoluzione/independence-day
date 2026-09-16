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
    /// <summary>Ogni quanto si guarda quali cannoni hanno finito di ricaricare.</summary>
    public int TickMs { get; set; } = 100;

    public int ReloadMs { get; set; } = Armory.ReloadMs;
}

/// <summary>
/// La centrale di tiro: preme il grilletto ai cannoni che hanno finito di ricaricare.
/// </summary>
/// <remarks>
/// La cadenza sta qui e non nell'aggregato per la stessa ragione per cui il tempo di avvicinamento
/// sta in <see cref="ApproachDeadline"/>: quanto ci mette una cosa a succedere lo conosce chi la
/// esegue. Chi coordina apre e chiude il fuoco, e non ha niente da sapere sulla ricarica.
/// <para>
/// E' anche il motivo per cui un cannone lasciato aperto continua a sparare senza che nessuno glielo
/// chieda: e' questo giro, non un ordine.
/// </para>
/// </remarks>
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

                    // La correlazione e' quella della nave presa di mira, cosi' l'esito torna a chi
                    // quel fuoco l'ha aperto. Se la nave non c'e' piu' il colpo si perde comunque, e
                    // la riga finisce nel diario sotto l'ondata in corso.
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
                // Un giro mancato non deve zittire i cannoni: si riprova al tick dopo.
                logger.LogError(ex, "[Earth] Giro della centrale di tiro fallito");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
