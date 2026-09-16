using Muflone.Core;

namespace Evoluzione.IndependenceDay.Space.Messages.DomainIds;

public sealed class CityId(Guid value) : DomainId(value.ToString());
