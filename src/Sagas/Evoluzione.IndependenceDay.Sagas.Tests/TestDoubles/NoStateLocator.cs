using System.Collections.Concurrent;
using Muflone;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.Persistence;
using Muflone.Saga;
using Muflone.Saga.Persistence;

namespace Evoluzione.IndependenceDay.Sagas.Tests.TestDoubles;

/// <summary>
/// Nessuna ricerca per chiave di business: i test indirizzano sempre per correlationId.
/// </summary>
public sealed class NoStateLocator : ISagaStateLocator
{
    public Task<IReadOnlyList<TSagaState>> FindOpenStatesBy<TSagaState>(string stateField, Guid value,
        CancellationToken ct = default) where TSagaState : SagaStateBase, new() =>
        Task.FromResult<IReadOnlyList<TSagaState>>([]);
}
