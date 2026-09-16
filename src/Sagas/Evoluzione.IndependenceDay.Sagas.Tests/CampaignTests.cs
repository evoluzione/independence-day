using Evoluzione.IndependenceDay.Sagas.Tests.TestDoubles;
using Xunit;
using Xunit.Abstractions;

namespace Evoluzione.IndependenceDay.Sagas.Tests;

/// <summary>
/// La campagna intera, giocata in memoria.
/// </summary>
/// <remarks>
/// Questi due test sono la rete del bilanciamento. Il primo dice che il gioco si puo' vincere; il
/// secondo dice che <b>non</b> si vince senza restituire i cannoni. Se il secondo diventa verde
/// significa che la compensazione e' diventata decorativa, e i numeri di <c>Armory</c> o della curva
/// di difficolta' vanno ristretti.
/// <para>
/// Il primo e' anche il contatore dei gradini: <c>CleanThrough</c> dice fin dove si e' arrivati senza
/// perdere una citta', ed e' il livello dell'ultimo gradino diventato verde. Finche' non lo sono
/// tutti, il messaggio di fallimento dice a che livello ci si e' fermati — cioe' quali test
/// guardare.
/// </para>
/// </remarks>
public class CampaignTests(ITestOutputHelper output)
{
    [Fact]
    public void Chi_arriva_in_fondo_alla_scala_vince_la_campagna()
    {
        var campaign = new Campaign();
        var result = campaign.Play();

        output.WriteLine(result.ToString());
        foreach (var (level, landed, standing, rounds) in campaign.PerLevel)
            output.WriteLine($"  livello {level,2}: {landed} atterrate, {standing} citta', {rounds} colpi");

        Assert.Equal(5, campaign.CleanThrough);
        Assert.True(result.Won, $"la campagna doveva essere vinta: {result}");
        Assert.Equal(0, result.OpenCannons);
        Assert.Equal(0, result.OpenLines);
        Assert.Equal(0, result.Unattended);
        Assert.True(result.RoundsLeft > 0, "vincere con zero colpi rimasti vuol dire margine nullo");

        // Qualche colpo su un relitto e' inevitabile: anche i cessate il fuoco si perdono, e fra
        // l'ordine perso e il battito che lo rivela passa un po' di fuoco. Quello che conta e' che
        // restino pochi, cioe' che ci si accorga in fretta.
        Assert.True(result.RoundsWasted < 20, $"troppi colpi su relitti: {result}");
    }

    [Fact]
    public void Chi_non_restituisce_i_cannoni_perde()
    {
        var result = new Campaign { Compensates = false }.Play();
        output.WriteLine(result.ToString());

        Assert.False(result.Won,
            $"senza compensazione la campagna deve perdersi, altrimenti restituire i cannoni non conta: {result}");
    }
}
