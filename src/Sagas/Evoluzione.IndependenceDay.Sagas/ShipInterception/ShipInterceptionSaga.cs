using Evoluzione.IndependenceDay.Contracts.Commands;
using Evoluzione.IndependenceDay.Contracts.Commands.Sagas;
using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Contracts.Ids;
using Evoluzione.IndependenceDay.Contracts.World;
using Microsoft.Extensions.Logging;
using Muflone.CustomTypes;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.Persistence;
using Muflone.Saga;
using Muflone.Saga.Persistence;

namespace Evoluzione.IndependenceDay.Sagas.ShipInterception;

public sealed class ShipInterceptionSaga(
    IServiceBus serviceBus,
    ISagaRepository repository,
    ISagaStateLocator stateLocator,
    ILoggerFactory loggerFactory)
    : LifecycleSaga<StartShipInterception, InterceptionState>(serviceBus, repository, stateLocator, loggerFactory),
        ISagaEventHandlerAsync<ShipApproaching>,
        ISagaEventHandlerAsync<FireOpened>,
        ISagaEventHandlerAsync<FireCeased>,
        ISagaEventHandlerAsync<CannonJammed>,
        ISagaEventHandlerAsync<CannonRepaired>,
        ISagaEventHandlerAsync<CannonEmpty>,
        ISagaEventHandlerAsync<CannonResupplied>,
        ISagaEventHandlerAsync<ShipDestroyed>,
        ISagaEventHandlerAsync<ShipLanded>
{
    private static readonly Account Coordinator = new("saga", "Ship Interception");

    private static readonly EarthId Earth = new(Cities.DefenseId);

    public override Task StartedByAsync(StartShipInterception command) =>
        SaveState(command.CorrelationId, new InterceptionState
        {
            CorrelationId = command.CorrelationId,
            Status = SagaStatus.InProgress
        });

    public Task HandleAsync(ShipApproaching @event) => Advance(@event, _ => []);

    public Task HandleAsync(ShipDestroyed @event) => Advance(@event, _ => []);

    public Task HandleAsync(ShipLanded @event) => Advance(@event, _ => []);

    public Task HandleAsync(FireOpened @event) => Advance(@event, _ => []);

    public Task HandleAsync(FireCeased @event) => Advance(@event, _ => []);

    public Task HandleAsync(CannonJammed @event) => Advance(@event, _ => []);

    public Task HandleAsync(CannonRepaired @event) => Advance(@event, _ => []);

    public Task HandleAsync(CannonEmpty @event) => Advance(@event, _ => []);

    public Task HandleAsync(CannonResupplied @event) => Advance(@event, _ => []);

    private async Task Advance(Event @event, Func<Guid, List<Command>> decide)
    {
        var correlationId = @event.Headers.CorrelationId;

        var state = await LoadState(correlationId);
        if (state is null || !TryAcceptEvent(state, @event))
            return;

        await SaveState(correlationId, state);

        foreach (var order in decide(correlationId))
            await SendCommand(order, correlationId);
    }

    private static Command OpenFire(ShipId shipId, Guid correlationId) =>
        new OpenFire(Earth, shipId, correlationId, Coordinator);

    private static Command CeaseFire(ShipId shipId, Guid correlationId) =>
        new CeaseFire(Earth, shipId, correlationId, Coordinator);

    private static Command Repair(CityId cityId, ShipId shipId, Guid correlationId) =>
        new RepairCannon(Earth, cityId, shipId, correlationId, Coordinator);

    private static Command Resupply(CityId cityId, ShipId shipId, Guid correlationId) =>
        new RequestResupply(Earth, cityId, shipId, correlationId, Coordinator);
}
