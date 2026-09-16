using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Contracts.Ids;
using Evoluzione.IndependenceDay.Contracts.World;
using Evoluzione.IndependenceDay.Sagas.Facade.IntegrationEventHandlers;
using Evoluzione.IndependenceDay.Sagas.Infrastructure.MongoDB;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Evoluzione.IndependenceDay.Sagas.Tests;

/// <summary>
/// Le linee della sala operativa tornano libere quando comincia una campagna nuova.
/// </summary>
/// <remarks>
/// Non e' un gradino dell'esercizio: e' una regola del campo di gioco, e sta qui perche' e' l'unica
/// che il simulatore non puo' vedere. Una campagna in memoria parte sempre da una sala vuota, quindi
/// il guasto — linee occupate da processi di una partita precedente — esiste soltanto nel gioco vero,
/// e si presenta nel modo peggiore: tutte le citta' rase al suolo alla prima ondata, senza un solo
/// ordine spedito e senza un errore da nessuna parte.
/// </remarks>
public class OperationsRoomTests
{
    private sealed class Room : IOperationsRoom
    {
        public int Freed;

        public Task<long> OpenLines(CancellationToken ct = default) => Task.FromResult(0L);

        public Task FreeLines(CancellationToken ct = default)
        {
            Freed++;

            return Task.CompletedTask;
        }
    }

    private static InvasionStarted Wave(int level) =>
        new(new InvasionId(Invasion.Id), level, level, 5, Guid.NewGuid());

    private static InvasionStartedIntegrationEventHandler Handler(Room room) =>
        new(room, new NullLoggerFactory());

    [Fact]
    public async Task Una_campagna_nuova_libera_tutte_le_linee()
    {
        var room = new Room();

        await Handler(room).HandleAsync(Wave(1));

        Assert.Equal(1, room.Freed);
    }

    /// <summary>Fra un livello e l'altro non si ripristina niente, qui come per le citta'.</summary>
    [Fact]
    public async Task Le_ondate_successive_non_liberano_niente()
    {
        var room = new Room();

        await Handler(room).HandleAsync(Wave(2));
        await Handler(room).HandleAsync(Wave(5));

        Assert.Equal(0, room.Freed);
    }
}
