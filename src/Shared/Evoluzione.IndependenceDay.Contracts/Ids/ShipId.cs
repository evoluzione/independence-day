using Muflone.Core;

namespace Evoluzione.IndependenceDay.Contracts.Ids;

public sealed class ShipId(Guid value) : DomainId(value.ToString());
