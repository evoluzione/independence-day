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
    /// <summary>
    /// Ogni quanto la Terra racconta come stanno le cose.
    /// </summary>
    /// <remarks>
    /// Mezzo secondo su otto di finestra. Il battito e' l'unico orologio di chi coordina, quindi e'
    /// anche il tempo che passa fra un ordine perso e il momento in cui si puo' rimediare:
    /// abbastanza stretto da poter rimediare, abbastanza largo da rendere conveniente <b>non</b>
    /// aspettarlo quando un evento dice gia' tutto quello che serve sapere.
    /// </remarks>
    public int TickMs { get; set; } = 500;
}

/// <summary>
/// Il battito: a intervalli regolari la Terra dice cosa sta succedendo, senza che nessuno lo chieda.
/// </summary>
/// <remarks>
/// E' l'unico orologio di chi coordina, e serve a vedere le due cose che nessun evento racconta,
/// perche' sono <b>assenze</b>:
/// <list type="bullet">
/// <item>una nave in volo a cui non spara nessuno — l'ordine di aprire il fuoco si e' perso;</item>
/// <item>un cannone che spara a una nave che non c'e' piu' — il cessate il fuoco si e' perso;</item>
/// <item>un cannone ancora inceppato — la riparazione non e' arrivata, o non ha preso.</item>
/// </list>
/// <para>
/// Non passa dagli aggregati: non e' un fatto di dominio, e' un resoconto. Lo si legge dal read
/// model e lo si mette sul bus com'e'.
/// </para>
/// </remarks>
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
                var alive = inFlight.Select(s => s.Id).ToHashSet();

                foreach (var ship in inFlight)
                    await bus.PublishAsync(
                        new ShipApproaching(Earth, new ShipId(ship.Id), new CityId(ship.CityId),
                            Math.Max(0, approachMs - (int)(now - ship.DetectedAt).TotalMilliseconds),
                            cannons.Count(c => c.Target == ship.Id),
                            ship.CorrelationId),
                        stoppingToken);

                // Cannoni rimasti puntati su navi che non ci sono piu': ogni giro e' un colpo buttato.
                foreach (var cannon in cannons.Where(c => c.Target != Guid.Empty && !alive.Contains(c.Target)))
                {
                    var ship = await battle.Ship(cannon.Target, stoppingToken);
                    if (ship is null)
                        continue;

                    await bus.PublishAsync(
                        new CannonStillFiring(Earth, new CityId(cannon.Id), new ShipId(cannon.Target),
                            ship.CorrelationId),
                        stoppingToken);
                }

                // Cannoni ancora fermi: la riparazione non e' arrivata, o non ha preso. Non c'e' modo
                // di distinguere i due casi, e non serve: la risposta e' la stessa.
                foreach (var cannon in await battle.JammedCannons(stoppingToken))
                {
                    if (cannon.JammedOn == Guid.Empty)
                        continue;

                    var ship = await battle.Ship(cannon.JammedOn, stoppingToken);
                    if (ship is null)
                        continue;

                    await bus.PublishAsync(
                        new CannonStillJammed(Earth, new CityId(cannon.Id), new ShipId(cannon.JammedOn),
                            ship.CorrelationId),
                        stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                // Un battito mancato non deve fermare la partita: si riprova al tick dopo.
                logger.LogError(ex, "[Earth] Battito fallito");
            }
        } while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
