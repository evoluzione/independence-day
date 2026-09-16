using Evoluzione.IndependenceDay.Earth.Messages.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Evoluzione.IndependenceDay.Earth.Facade.BackgroundServices;

/// <summary>
/// Mette in piedi la difesa all'avvio, una volta sola.
/// </summary>
/// <remarks>
/// Il comando parte a ogni avvio ma il suo handler non fa nulla se lo stream esiste gia': e' li' che
/// sta l'idempotenza, non qui. Il ripristino fra una campagna e l'altra lo fa <c>RecommissionEarth</c>.
/// </remarks>
public class EarthSeeder(IServiceScopeFactory scopeFactory, ILogger<EarthSeeder> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = scopeFactory.CreateScope();

        await scope.ServiceProvider.GetRequiredService<IServiceBus>().SendAsync(
            new CommissionEarth(EarthDefenses.Id, EarthDefenses.Rounds, EarthDefenses.Integrity, Guid.NewGuid(),
                EarthDefenses.HighCommand),
            stoppingToken);

        logger.LogInformation("[Earth] Difesa in piedi: cinque citta', cinque cannoni");
    }
}
