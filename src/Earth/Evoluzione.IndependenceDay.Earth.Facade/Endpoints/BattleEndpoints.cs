using System.Text.Json;
using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Contracts.Ids;
using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Earth.ReadModel;
using Muflone.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Evoluzione.IndependenceDay.Earth.Facade.Endpoints;

public static class BattleEndpoints
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static IEndpointRouteBuilder MapBattleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/earth").WithTags("Battle");

        group.MapGet("/battle", async (IBattleService battle, CancellationToken ct) =>
            Results.Ok(await battle.GetSnapshot(ct)));

        // I due pulsanti della sala operativa. Sono comandi di simulazione, non atti di dominio della
        // Terra: partono da qui perche' qui sta l'operatore, e raggiungono lo Spazio sul bus come ogni
        // altro messaggio (R-9000).
        //
        // Sono due e non uno perche' le due cose sono diverse: una campagna nuova riporta le difese a
        // nuovo, l'ondata successiva no. E' quella differenza a fare il gioco.
        group.MapPost("/campaign/start", async (IServiceBus serviceBus, CancellationToken ct) =>
        {
            await serviceBus.SendAsync(
                new StartCampaign(new InvasionId(Invasion.Id), Guid.NewGuid(), EarthDefenses.HighCommand), ct);

            return Results.Accepted();
        });

        group.MapPost("/campaign/next-wave", async (IServiceBus serviceBus, CancellationToken ct) =>
        {
            await serviceBus.SendAsync(
                new StartNextWave(new InvasionId(Invasion.Id), Guid.NewGuid(), EarthDefenses.HighCommand), ct);

            return Results.Accepted();
        });

        // Server-sent events: la pagina non interroga, viene svegliata. Ogni evento proiettato suona
        // la campanella e qui si rilegge lo snapshot e lo si spinge giu'.
        group.MapGet("/stream", async (HttpContext context, IBattleService battle, BattleFeed feed,
            CancellationToken ct) =>
        {
            context.Response.Headers.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            // Senza questo un reverse proxy puo' tenersi lo stream in buffer e la pagina non vede nulla.
            context.Response.Headers["X-Accel-Buffering"] = "no";

            await Push(context, battle, ct);

            // Il battito tiene viva la connessione anche nei minuti in cui non succede niente, e
            // rimedia a un avviso perso mentre la pagina era ancora in fase di aggancio.
            using var heartbeat = new PeriodicTimer(TimeSpan.FromSeconds(10));
            var ticks = feed.Subscribe(ct);
            var beat = heartbeat.WaitForNextTickAsync(ct).AsTask();
            var enumerator = ticks.GetAsyncEnumerator(ct);
            var next = enumerator.MoveNextAsync().AsTask();

            try
            {
                while (!ct.IsCancellationRequested)
                {
                    var completed = await Task.WhenAny(next, beat);
                    if (completed == next)
                    {
                        if (!await next)
                            break;
                        next = enumerator.MoveNextAsync().AsTask();
                    }
                    else
                    {
                        if (!await beat)
                            break;
                        beat = heartbeat.WaitForNextTickAsync(ct).AsTask();
                    }

                    await Push(context, battle, ct);
                }
            }
            catch (OperationCanceledException)
            {
                // La pagina si e' chiusa: niente da segnalare.
            }
            finally
            {
                await enumerator.DisposeAsync();
            }
        });

        return app;
    }

    private static async Task Push(HttpContext context, IBattleService battle, CancellationToken ct)
    {
        var snapshot = await battle.GetSnapshot(ct);

        await context.Response.WriteAsync($"data: {JsonSerializer.Serialize(snapshot, Json)}\n\n", ct);
        await context.Response.Body.FlushAsync(ct);
    }
}
