using Muflone.Core;

namespace Evoluzione.IndependenceDay.Earth.Messages.DomainIds;

public sealed class ShipId(Guid value) : DomainId(value.ToString());
