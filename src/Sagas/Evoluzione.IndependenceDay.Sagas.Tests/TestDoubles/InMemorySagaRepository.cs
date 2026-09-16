using System.Collections.Concurrent;
using Muflone;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.Persistence;
using Muflone.Saga;
using Muflone.Saga.Persistence;

namespace Evoluzione.IndependenceDay.Sagas.Tests.TestDoubles;

public sealed class InMemorySagaRepository : ISagaRepository
{
    private readonly ConcurrentDictionary<Guid, object> _states = new();

    public bool Contains(Guid correlationId) => _states.ContainsKey(correlationId);

    /// <summary>Quante intercettazioni sono aperte adesso: e' quello che occupa la sala operativa.</summary>
    public int Open => _states.Count;

    public TSagaState? Peek<TSagaState>(Guid correlationId) where TSagaState : class, new() =>
        _states.TryGetValue(correlationId, out var state) ? state as TSagaState : null;

    public Task SaveAsync<TSagaState>(Guid correlationId, TSagaState sagaState) where TSagaState : class, new()
    {
        _states[correlationId] = sagaState;

        return Task.CompletedTask;
    }

    public Task<TSagaState> GetByIdAsync<TSagaState>(Guid id) where TSagaState : class, new() =>
        Task.FromResult((_states.TryGetValue(id, out var state) ? state as TSagaState : null)!);

    public Task CompleteAsync(Guid correlationId)
    {
        _states.TryRemove(correlationId, out _);

        return Task.CompletedTask;
    }
}
