using Evoluzione.IndependenceDay.Space.ReadModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Evoluzione.IndependenceDay.Space.Facade.Endpoints;

public static class ShipsEndpoints
{
    public static IEndpointRouteBuilder MapShipsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/space/ships").WithTags("Ships");

        group.MapGet("/", async (IShipsService ships, CancellationToken ct) =>
            Results.Ok(await ships.GetAll(ct)));

        group.MapPost("/", async (LaunchShipRequest request, ISpaceFacade facade,
            IInvasionProgressService invasion, CancellationToken ct) =>
        {
            var progress = await invasion.Current(Contracts.World.Invasion.Id, ct);
            if (progress is null || !progress.Running)
                return Results.Conflict(new { error = "Nessuna ondata in corso: l'invasione non e' partita." });

            var shipId = await facade.LaunchShip(request.TargetCityId, request.ShipClass, progress.Wave, ct);

            return Results.Accepted($"/space/ships/{shipId}", new { shipId });
        });

        return app;
    }
}

public record LaunchShipRequest(Guid TargetCityId, Contracts.World.ShipClass ShipClass);
