namespace Evoluzione.IndependenceDay.Space.Domain.Entities;

public enum ShipStatus
{
    /// <summary>In avvicinamento: e' l'unico stato da cui la nave puo' ancora cambiare destino.</summary>
    Approaching = 0,

    Destroyed = 1,

    /// <summary>Atterrata. La citta' bersaglio e' persa.</summary>
    Landed = 2
}
