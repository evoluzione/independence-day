using Muflone.Core;

namespace Evoluzione.IndependenceDay.Earth.ReadModel.EventHandlers;

/// <summary>
/// Il montaggio comune di ogni proiezione della Terra.
/// </summary>
/// <remarks>
/// Quattordici proiezioni facevano quattordici volte le stesse tre cose: aggiornare, avvisare la
/// pagina, e non rilanciare. L'ultima non e' un vezzo — una proiezione che rilancia blocca la
/// sottoscrizione per <b>tutti</b> gli eventi dopo di lei, e la partita si ferma senza che a schermo
/// si veda altro che uno schermo fermo.
/// </remarks>
public abstract class EarthProjection<TEvent>(
    IBattleService battle,
    BattleFeed feed,
    ILoggerFactory loggerFactory) : DomainEventHandlerAsync<TEvent>(loggerFactory)
    where TEvent : DomainEvent
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<TEvent>();

    protected IBattleService Battle { get; } = battle;

    protected abstract Task Project(TEvent @event, CancellationToken ct);

    protected static Guid Id(IDomainId id) => Guid.Parse(id.Value);

    public override async Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            await Project(@event, cancellationToken);
            feed.Notify();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Earth] Read model non aggiornato: {Event}", typeof(TEvent).Name);
        }
    }
}
