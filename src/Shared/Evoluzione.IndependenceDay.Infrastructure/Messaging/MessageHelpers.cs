using Muflone.Messages.Events;

namespace Evoluzione.IndependenceDay.Infrastructure.Messaging;

public static class MessageHelpers
{
    public const long Unversioned = -1;

    public static DateTime When(this Event @event) => @event.Headers.When.Value;

    public static Guid CorrelationId(this Event @event) => @event.Headers.CorrelationId;

    public static long EventRevision(this Event @event)
    {
        if (@event.Headers.Customs.TryGetValue("EventStorePreparePosition", out var prepare) &&
            long.TryParse(prepare, out var preparePosition))
            return preparePosition;

        if (@event.Headers.Customs.TryGetValue("EventStoreCommitPosition", out var commit) &&
            long.TryParse(commit, out var commitPosition))
            return commitPosition;

        return Unversioned;
    }
}
