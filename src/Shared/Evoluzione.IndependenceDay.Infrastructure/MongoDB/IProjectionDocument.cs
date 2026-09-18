namespace Evoluzione.IndependenceDay.Infrastructure.MongoDB;

public interface IProjectionDocument
{
    Guid Id { get; set; }
    DateTime UpdatedAt { get; set; }

    long ProjectionVersion { get; set; }
}
