using Muflone.Core;

namespace Evoluzione.IndependenceDay.Space.Messages.DomainIds;

public sealed class MotherShipId(Guid value) : DomainId(value.ToString());
