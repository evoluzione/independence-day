# Le regole

Tutti i numeri qui sotto stanno in chiaro nel codice, in
`src/Shared/Evoluzione.IndependenceDay.Contracts/World/`. Non c'è niente di nascosto e niente di
casuale: **i guasti sono deterministici**, nessun dado, nessuna probabilità.

## La Terra

| | |
| --- | --- |
| Città | **5**, un cannone ciascuna |
| Colpi | **50 per città**, 250 in tutto. Non si ricaricano **mai**, né fra un'ondata e l'altra |
| Cadenza | un colpo ogni **350 ms** |
| Bersagli | **uno alla volta**: un cannone spara a una nave sola |
| Raggio | tutte le città sparano a tutte le navi, non solo a quella sopra di loro |

**Quale cannone spari non lo decidi tu.** Chiedi di aprire il fuoco su una nave, e la Terra sceglie:
prende il più **scarico** fra quelli liberi. Consuma prima le riserve piccole e tiene indietro quelle
piene — con il prezzo che ne segue, e che è la regola del quinto livello.

**Un colpo su tre manca il bersaglio.** La munizione se ne va e la nave regge. Non c'è niente da fare
e non te lo dice nessuno: il fuoco è aperto, quindi il cannone ricarica e riprova da solo.

## Il fuoco continuato

Un ordine di apertura non è un colpo: **accende** un cannone, che da lì spara da solo finché non gli
si dice di smettere. **Non si ferma quando la nave cade.** Fermarlo è un ordine a parte —
`CeaseFire` — e va confermato.

## Le navi

Ogni attacco è una nave sola. Quanti colpi servano ad abbatterla **non lo dice nessun evento**: lo sa
solo la Terra. Lo scopri perché la nave cade.

| Stazza | Colpi a segno |
| --- | ---: |
| 🛸 Caccia | 1 |
| 🛰️ Incrociatore | 4 |
| 🚀 Corazzata | 9 |

**Una nave che tocca terra rade al suolo la città**, qualunque stazza. Con la città se ne vanno il
cannone e i colpi che gli restavano: un quinto della potenza di fuoco per tutto il resto della
campagna.

Dalla presa in carico hai **8 secondi**. Poi tocca terra.

## I quattro guasti

### 1. L'ordine perso — uno su venticinque

Non arriva alla Terra. Non succede niente, quindi **non torna indietro nessun evento** — nemmeno un
rifiuto. Vale per tutti e tre gli ordini. L'unico modo di accorgersene è il battito.

Due ordini di fila non si perdono mai: riprovare basta sempre.

### 2. L'inceppamento — ogni quattordici grilletti

Il colpo non parte, non consuma munizioni, e il cannone si ferma. Il conto è sui **grilletti
premuti**, colpi mancati e colpi su relitti compresi.

Non si sblocca da solo: senza `RepairCannon` è perso per il resto della campagna.

### 3. La riparazione che non prende — una su due

Riparare costa **3 colpi a tentativo** e non riapre il fuoco: rimette il cannone disponibile, e
fermo. Ma **una riparazione su due non prende**: i colpi se ne vanno e il cannone resta inceppato.

La Terra lo dice — `CannonStillJammed` — quindi è un evento, non un silenzio. Due tentativi di fila
non falliscono mai.

### 4. Il cannone a secco

Un cannone che finisce i colpi **resta assegnato alla sua nave** finché non lo si restituisce. Non
spara più, ma per il battito quella nave risulta coperta.

## Il battito — ogni mezzo secondo

La Terra racconta come stanno le cose senza che nessuno lo chieda. Serve a vedere le tre cose che
**nessun evento** può raccontare, perché sono assenze:

| Battito | Vuol dire |
| --- | --- |
| `ShipApproaching` con **zero cannoni** | l'apertura del fuoco si è persa |
| `CannonStillFiring` | il cessate il fuoco si è perso |
| `CannonStillJammed` | la riparazione si è persa, o non ha preso |

È l'unico orologio che hai — ed è una rete, non il meccanismo: quando un evento dice già tutto,
aspettare il battito costa mezzo secondo su otto di finestra.

## La sala operativa

**30 linee.** Un processo aperto ne occupa una, e la libera **solo chiudendosi**.

Le linee tornano tutte libere quando comincia una campagna **nuova**, come le città. Dentro la stessa
campagna non torna indietro niente.

Sono molte più delle navi che possono essere in volo insieme, quindi non è una risorsa da dosare: è
la ragione per cui chiudere un processo è una mossa. Un processo che non si chiude mai non fa male
subito — tiene la sua linea, e basta. Ma le navi passano a decine e le linee non tornano indietro:
a un certo punto una nave viene avvistata e nessuno la prende in carico. Nessun ordine, nessun
cannone, nessun errore.

## I cinque livelli

Una nave parte ogni **secondo** e punta le città **a turno**. Le più pesanti partono per prime: chi
arriva dopo trova i cannoni già impegnati.

| Livello | Navi | 🛸 | 🛰️ | 🚀 | Colpi a segno | Cumulato |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | 5 | 5 | 0 | 0 | 5 | 5 |
| 2 | 6 | 5 | 1 | 0 | 9 | 14 |
| 3 | 7 | 5 | 2 | 0 | 13 | 27 |
| 4 | 8 | 4 | 3 | 1 | 25 | 52 |
| 5 | 9 | 3 | 4 | 2 | 37 | **89** |

Trentacinque navi in tutto. Con un colpo su tre a vuoto, gli 89 colpi a segno sono circa **134
grilletti** su 250 in dotazione, meno quelli spesi nelle riparazioni. Il margine c'è: la soluzione di
riferimento chiude il quinto livello con cinque città in piedi, nessuna nave a terra e circa **88
colpi** avanzati.

Il vincolo vero però non sono i colpi: sono i **cannoni liberi**. All'ultimo livello le corazzate
partono per prime, e chi arriva dopo aspetta. Un cannone lasciato acceso su un relitto non è un colpo
sprecato: è un posto vuoto in quella fila.

## I cinque gradini

I test di `Sagas.Tests` sono cinque gradini, e **ognuno è un livello**: con i gradini fino a N verdi
la campagna arriva in fondo al livello N senza perdere una città.

È una garanzia sul minimo, non sul massimo. Un gradino scritto bene può portarti anche un po' più in
là, ma nessuno dei cinque è superfluo, e saltarne uno ti ferma dove quel gradino serviva.

| Livello | Gradino | Quello che serve | Perché lì |
| ---: | --- | --- | --- |
| 1 | aprire il fuoco | `StartedBy` → `OpenFire` | senza, non spara nessuno |
| 2 | restituire il cannone | `ShipDestroyed` → `CeaseFire`, e il conto di quello che hai aperto | le navi diventano più dei cannoni |
| 3 | insistere quando c'è silenzio | `ShipApproaching` con zero cannoni → `OpenFire`; `CannonStillFiring` → `CeaseFire` | si perdono i primi ordini |
| 4 | riparare, e rimettere in azione | `CannonJammed` e `CannonStillJammed` → `RepairCannon`; `CannonRepaired` → `OpenFire` | arrivano i primi inceppamenti |
| 5 | chiudere il conto | `CompleteSaga` solo a conto saldato; `CannonEmpty` → `CeaseFire` + `OpenFire` | finiscono le linee e i cannoni |

C'è un branch per gradino, `livello-01` … `livello-05`, ognuno con la soluzione fino a quel livello.
Il diff fra due consecutivi è esattamente quello che aggiunge il gradino:

```bash
git diff livello-03 livello-04 -- src/Sagas/
```

## Come si vince

Superare il livello 5. La campagna è persa quando cade l'ultima città.

A parità di vittoria contano, in quest'ordine: le **città in piedi**, le **navi atterrate**, i
**colpi non spesi**.

## Cosa si può toccare

**Sì** a tutto quello che sta sotto `src/Sagas/`: la saga, il suo stato, il processo, le
registrazioni.

**No** agli aggregati, ai guasti e alla curva di difficoltà: sono il campo di gioco, uguale per
tutti.

Un processo vive per **una nave**, non per la città. Non sa cosa stanno facendo gli altri, non sa
quanti colpi restano, non sa quali cannoni sono liberi. Se ti serve saperlo, ricavalo dagli eventi.
