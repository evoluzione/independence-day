using Muflone.Core;

namespace Evoluzione.IndependenceDay.Earth.Messages.DomainIds;

public sealed class CityId(Guid value) : DomainId(value.ToString());
