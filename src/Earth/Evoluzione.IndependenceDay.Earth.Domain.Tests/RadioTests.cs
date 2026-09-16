using Evoluzione.IndependenceDay.Contracts.World;
using Xunit;

namespace Evoluzione.IndependenceDay.Earth.Domain.Tests;

/// <summary>
/// La regola del collegamento, che non e' dominio ma decide come si gioca.
/// </summary>
/// <remarks>
/// Due proprieta', e servono entrambe. Che ogni tanto un ordine si perda e' la sfida; che <b>due di
/// fila non si perdano mai</b> e' quello che rende insistere una risposta sensata invece di un giro
/// infinito. Senza la seconda il gioco non sarebbe difficile, sarebbe rotto.
/// </remarks>
public class RadioTests
{
    [Fact]
    public void Un_ordine_su_venticinque_si_perde()
    {
        var persi = Enumerable.Range(1, Radio.LostEveryOrders * 10).Count(n => !Radio.Delivers(n));

        Assert.Equal(10, persi);
    }

    [Fact]
    public void Due_ordini_di_fila_non_si_perdono_mai()
    {
        var consecutivi = Enumerable.Range(1, 1000).Any(n => !Radio.Delivers(n) && !Radio.Delivers(n + 1));

        Assert.False(consecutivi, "riprovare deve bastare sempre");
    }

    /// <summary>Il primo ordine di una partita deve arrivare: vederlo sparire sembra un guasto del gioco.</summary>
    [Fact]
    public void Il_primo_ordine_arriva()
    {
        Assert.True(Radio.Delivers(1));
    }
}
