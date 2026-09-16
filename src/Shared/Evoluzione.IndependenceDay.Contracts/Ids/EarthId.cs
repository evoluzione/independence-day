using Muflone.Core;

namespace Evoluzione.IndependenceDay.Contracts.Ids;

public sealed class EarthId(Guid value) : DomainId(value.ToString());
