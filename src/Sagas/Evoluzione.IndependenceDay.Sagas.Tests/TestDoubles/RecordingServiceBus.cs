using System.Collections.Concurrent;
using Muflone;
using Muflone.Messages.Commands;
using Muflone.Messages.Events;
using Muflone.Persistence;
using Muflone.Saga;
using Muflone.Saga.Persistence;

namespace Evoluzione.IndependenceDay.Sagas.Tests.TestDoubles;

/// <summary>Raccoglie i comandi invece di spedirli: e' quello che i test osservano.</summary>
public sealed class RecordingServiceBus : IServiceBus
{
    public List<object> Sent { get; } = [];

    public Task SendAsync<T>(T command, CancellationToken cancellationToken = default) where T : class, ICommand
    {
        Sent.Add(command!);

        return Task.CompletedTask;
    }

    public IEnumerable<T> SentOf<T>() => Sent.OfType<T>();
}
