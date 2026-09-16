namespace Evoluzione.IndependenceDay.Infrastructure.MongoDB;

/// <summary>
/// Un documento di read model che sa a quale punto del log e' arrivato.
/// </summary>
public interface IProjectionDocument
{
    Guid Id { get; set; }
    DateTime UpdatedAt { get; set; }

    /// <summary>La posizione sul log dell'ultimo evento applicato.</summary>
    long ProjectionVersion { get; set; }
}
