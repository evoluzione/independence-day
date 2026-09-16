using Muflone.Core;

namespace Evoluzione.IndependenceDay.Space.Messages.DomainIds;

public sealed class InvasionId(Guid value) : DomainId(value.ToString());
