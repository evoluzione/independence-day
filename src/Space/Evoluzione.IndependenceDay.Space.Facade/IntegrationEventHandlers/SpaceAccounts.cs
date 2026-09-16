using Evoluzione.IndependenceDay.Contracts.Events;
using Evoluzione.IndependenceDay.Infrastructure.Messaging;

namespace Evoluzione.IndependenceDay.Space.Facade.IntegrationEventHandlers;

internal static class SpaceAccounts
{
    public static readonly Account Earth = new("earth", "Earth Defense");
}
