using Muflone.Core;

namespace Evoluzione.IndependenceDay.Contracts.Ids;

public sealed class CityId(Guid value) : DomainId(value.ToString());
