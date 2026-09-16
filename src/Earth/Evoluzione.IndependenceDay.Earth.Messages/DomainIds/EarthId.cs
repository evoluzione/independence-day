using Muflone.Core;

namespace Evoluzione.IndependenceDay.Earth.Messages.DomainIds;

public sealed class EarthId(Guid value) : DomainId(value.ToString());
