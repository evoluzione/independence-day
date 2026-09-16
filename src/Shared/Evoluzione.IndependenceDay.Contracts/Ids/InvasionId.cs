using Muflone.Core;

namespace Evoluzione.IndependenceDay.Contracts.Ids;

public sealed class InvasionId(Guid value) : DomainId(value.ToString());
