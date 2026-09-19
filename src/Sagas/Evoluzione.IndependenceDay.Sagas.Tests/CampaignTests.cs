using Evoluzione.IndependenceDay.Sagas.Tests.TestDoubles;
using Xunit;
using Xunit.Abstractions;

namespace Evoluzione.IndependenceDay.Sagas.Tests;

public class CampaignTests(ITestOutputHelper output)
{
    [Fact]
    public void Ogni_comportamento_in_piu_abbatte_piu_navi_del_precedente()
    {
        Skills[] steps =
        [
            Skills.Fire,
            Skills.Fire | Skills.Cease,
            Skills.Fire | Skills.Cease | Skills.Repair,
            Skills.Fire | Skills.Cease | Skills.Repair | Skills.Resume,
            Skills.All
        ];

        var results = steps.Select(skills => new Campaign { Skills = skills }.Play()).ToList();

        foreach (var (skills, result) in steps.Zip(results))
            output.WriteLine($"{skills,-45} {result}");

        var destroyed = results.Select(r => r.ShipsDestroyed).ToList();

        for (var i = 1; i < destroyed.Count; i++)
            Assert.True(destroyed[i] > destroyed[i - 1],
                $"il gradino {steps[i]} doveva abbattere piu' navi del precedente: {destroyed[i - 1]} -> {destroyed[i]}");

        var last = results[^1];
        Assert.True(last.Won, $"con tutti i comportamenti la campagna doveva essere vinta: {last}");
        Assert.Equal(0, last.ShipsLanded);
        Assert.Equal(0, last.OpenCannons);
        Assert.Equal(0, last.RoundsWasted);
        Assert.True(last.RoundsLeft > 0, "vincere con zero colpi rimasti vuol dire margine nullo");
    }
}
