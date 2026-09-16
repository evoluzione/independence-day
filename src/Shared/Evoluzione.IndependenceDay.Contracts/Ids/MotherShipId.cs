using Muflone.Core;

namespace Evoluzione.IndependenceDay.Contracts.Ids;

public sealed class MotherShipId(Guid value) : DomainId(value.ToString());
