using Microsoft.Extensions.Logging.Abstractions;
using Muflone.CustomTypes;
using Muflone.Messages.Commands;
using Muflone.SpecificationTests;

namespace Evoluzione.IndependenceDay.Space.Domain.Tests;

public abstract class SpaceCommandSpecification<TCommand> : CommandSpecification<TCommand>
    where TCommand : Command
{
    protected static readonly Account Invaders = new("space", "Ship Command");

    protected const int Wave = 1;

    protected static NullLoggerFactory LoggerFactory { get; } = new();
}
