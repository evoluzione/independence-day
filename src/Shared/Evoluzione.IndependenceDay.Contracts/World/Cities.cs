namespace Evoluzione.IndependenceDay.Contracts.World;

public static class Cities
{
    public static readonly Guid DefenseId = Guid.Parse("22222222-0000-0000-0000-000000000000");

    public static readonly IReadOnlyList<(Guid Id, string Name)> All =
    [
        (Guid.Parse("11111111-0000-0000-0000-000000000001"), "New York"),
        (Guid.Parse("11111111-0000-0000-0000-000000000002"), "Los Angeles"),
        (Guid.Parse("11111111-0000-0000-0000-000000000003"), "Washington D.C."),
        (Guid.Parse("11111111-0000-0000-0000-000000000004"), "Houston"),
        (Guid.Parse("11111111-0000-0000-0000-000000000005"), "Area 51")
    ];

    public static string NameOf(Guid id) =>
    All.FirstOrDefault(city => city.Id == id).Name ?? string.Empty;
}
