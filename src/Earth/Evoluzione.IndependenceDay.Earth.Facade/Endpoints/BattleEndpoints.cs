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

        group.MapPost("/campaign/start", async (IServiceBus serviceBus, CancellationToken ct) =>
        {
            await serviceBus.SendAsync(
                new StartCampaign(new InvasionId(Invasion.Id), Guid.NewGuid(), EarthDefenses.HighCommand), ct);

            return Results.Accepted();
        });

        group.MapGet("/stream", async (HttpContext context, IBattleService battle, BattleFeed feed,
            CancellationToken ct) =>
        {
            context.Response.Headers.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";

            context.Response.Headers["X-Accel-Buffering"] = "no";

            await Push(context, battle, ct);

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
