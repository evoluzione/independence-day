using Muflone.Core;

namespace Evoluzione.IndependenceDay.Space.Messages.DomainIds;

public sealed class ShipId(Guid value) : DomainId(value.ToString());
