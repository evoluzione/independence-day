# R-9050: Un Apply non contiene condizioni

- Status: enforced
- Source: [R-9010](9010-aggregates-do-not-throw.md)

## Rule

Un metodo `Apply` di un aggregato è una transizione di stato incondizionata: assegna, e basta. Non
contiene `if`, non controlla se una chiave esiste, non decide se l'evento "vale".

Se un `Apply` ha bisogno di una condizione, il difetto non è lì: è **l'evento a essere povero**.
L'evento va arricchito finché non descrive da solo lo stato che produce.

Il caso di riferimento è `EarthUnitsEngaged`. Scritto così avrebbe bisogno di una guardia, perché
l'ingaggio potrebbe non esserci più:

```csharp
Ships[@event.ShipId.Value] = Ships[@event.ShipId.Value] with { Hits = ... };
```

Scritto così non ne ha bisogno, perché l'evento porta già città e navi rimaste:

```csharp
Ships[@event.ShipId.Value] = new Incoming(@event.CityId.Value, @event.Class, @event.Hits);
```

## Why

Un `Apply` che solleva un'eccezione non sbaglia un comando: rende impossibile **ricostruire
l'aggregato**. Da quel momento ogni comando muore in reidratazione, e lo stream non si riscrive — il
danno è definitivo. È successo: un `KeyNotFoundException` dentro `Apply(EarthUnitsEngaged)` ha spento
l'intera difesa della Terra, e da fuori sembrava che il gioco si fosse bloccato.

L'ordine degli eventi nello stream non è garantito quanto sembra. Ogni tipo di comando ha la sua coda,
quindi due comandi diversi lavorano sullo stesso aggregato nello stesso momento: l'atterraggio di una
nave e il colpo del cannone che la stava affrontando. Il colpo può finire nello stream **dopo**
l'atterraggio della stessa nave.

Mettere una guardia nell'`Apply` sembra la difesa, ma è il rimedio sbagliato: nasconde l'evento povero,
e lascia l'aggregato a ricostruirsi in modo diverso a seconda dell'ordine di arrivo. Un evento che si
applica da solo rende l'ordine irrilevante, che è la proprietà che serve davvero.

## Enforcement

Revisione: in un `Apply` la parola `if` è un difetto, non una scelta. Un `Apply` che indicizza una
raccolta deve poterlo fare sempre — le cinque città esistono dalla nascita dell'aggregato, e per tutto
il resto si assegna.
