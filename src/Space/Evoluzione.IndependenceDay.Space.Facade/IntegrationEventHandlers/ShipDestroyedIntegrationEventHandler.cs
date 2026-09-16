using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Infrastructure.Messaging;

namespace Evoluzione.IndependenceDay.Space.Facade.IntegrationEventHandlers;

/// <summary>
/// Il ritorno della Terra sullo Spazio: com'e' finita la nave.
/// </summary>
/// <remarks>
/// Lo Spazio non chiede niente a nessuno e non legge il read model della Terra: apprende l'esito dagli
/// eventi sul bus e lo traduce in un comando sul proprio aggregato. E' anche cosi' che sa quando
/// un'ondata e' finita — le sue navi non sono piu' in avvicinamento.
/// </remarks>
public class ShipDestroyedIntegrationEventHandler(IServiceBus serviceBus, ILoggerFactory loggerFactory)
    : IntegrationEventHandlerAsync<ShipDestroyed>(loggerFactory)


{
    public override Task HandleAsync(ShipDestroyed @event, CancellationToken cancellationToken = default) =>
        serviceBus.SendAsync(
            new DestroyAlienShip(new ShipId(Guid.Parse(@event.ShipId.Value)), "abbattuta",
                @event.CorrelationId(), SpaceAccounts.Earth),
            cancellationToken);
}
