# Le regole

Tutti i numeri qui sotto stanno in chiaro nel codice, in
`src/Shared/Evoluzione.IndependenceDay.Contracts/World/`. Non c'è niente di nascosto e niente di
casuale: **i guasti sono deterministici**, nessun dado, nessuna probabilità.

## La Terra

| | |
| --- | --- |
| Città | **5**, un cannone ciascuna |
| Colpi | **40 per città** all'inizio, 200 in tutto. Non si ricaricano da soli, ma un cannone a
secco si rifornisce |
| Cadenza | un colpo ogni **350 ms** |
| Bersagli | **uno alla volta**: un cannone spara a una nave sola |
| Raggio | tutte le città sparano a tutte le navi, non solo a quella sopra di loro |

**Quale cannone spari non lo decidi tu.** Chiedi di aprire il fuoco su una nave, e la Terra sceglie:
prende il più **scarico** fra quelli liberi. Consuma prima le riserve piccole e tiene indietro quelle
piene, così la potenza di fuoco che resta è concentrata invece che spalmata su cinque cannoni tutti
quasi a secco.

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

## I tre guasti

### 1. L'inceppamento — ogni dieci grilletti

Il colpo non parte, non consuma munizioni, e il cannone si ferma. Il conto è sui **grilletti
premuti**, colpi mancati e colpi su relitti compresi.

Non si sblocca da solo: senza `RepairCannon` è perso per il resto della campagna.

### 2. La riparazione

Riparare costa **3 colpi** e non riapre il fuoco: rimette il cannone disponibile, e fermo.
Rimetterlo in azione è una mossa a parte.

### 3. Il cannone a secco

Un cannone che finisce i colpi si libera: smette di essere assegnato alla sua nave, e chi la stava
intercettando resta scoperto. Non torna carico da solo — va chiamato il convoglio, `RequestResupply`
— e il convoglio ci mette **3 secondi** a portare **20 colpi**. Rimettere il cannone in azione, come
per la riparazione, è un ordine a parte: `CannonResupplied` non riapre il fuoco.

## Il battito — ogni mezzo secondo

La Terra racconta come stanno le cose senza che nessuno lo chieda. Serve a vedere l'unica cosa che
**nessun evento** può raccontare, perché è un'assenza: `ShipApproaching` con **zero cannoni** dice
che una nave è scoperta — perché tutti i cannoni erano impegnati quando è arrivata, non perché un
ordine si sia perso da qualche parte.

È l'unico orologio che hai — ed è una rete, non il meccanismo: quando un evento dice già tutto,
aspettare il battito costa mezzo secondo su otto di finestra.

## L'ondata

Una nave parte ogni **secondo**, ed è tutta la campagna: **36 navi**, un'unica ondata.

| 🛸 Caccia | 🛰️ Incrociatore | 🚀 Corazzata | Colpi a segno |
| ---: | ---: | ---: | ---: |
| 12 | 18 | 6 | 138 |

Non arrivano in blocco per stazza: sei blocchi identici, ognuno con **una corazzata, tre
incrociatori, due caccia** — le più pesanti in testa a ogni blocco. Se le sei corazzate arrivassero
tutte insieme inceppherebbero i cinque cannoni nei primi secondi, e senza riparazione un inceppamento
non passa mai: nessuna nave dopo, nemmeno un caccia che basta un colpo a fermare, troverebbe più un
cannone libero per tutta l'ondata. A blocchi, il peso è distribuito lungo tutta l'ondata.

Il conto dei colpi a segno (138) supera i 200 in dotazione una volta contati i colpi a vuoto e le
riparazioni — circa due colpi spesi per ogni colpo a segno. Il margine si assottiglia con l'ondata, ed
è la ragione per cui i rifornimenti contano: senza, l'ultimo terzo dell'ondata trova i cannoni a
secco.

## Le cinque mosse

I test di `Sagas.Tests` sono cinque, uno per problema. Non sono un cancello: ogni mossa in più
abbatte **più navi** di quella prima, e si vede nel resoconto di fine ondata.

| # | Problema | Senza |
| ---: | --- | --- |
| 1 | Aprire il fuoco all'avvistamento, e insistere se il battito dice che la nave è scoperta | non spara nessuno |
| 2 | Restituire il cannone quando la nave cade | il fuoco continua sul relitto, e il cannone manca alla nave dopo |
| 3 | Far riparare un cannone inceppato | un cannone su cinque è perso per sempre al primo inceppamento |
| 4 | Rimettere subito in azione un cannone appena riparato | il cannone resta fermo fino al battito successivo |
| 5 | Chiedere i rifornimenti per un cannone a secco, e riaprire il fuoco alla consegna | un cannone su cinque si ferma per sempre alla prima ricarica esaurita |

## Come si vince

Arrivare in fondo all'ondata con **almeno una città in piedi**. La campagna è persa quando cade
l'ultima città.

A parità di vittoria contano, in quest'ordine: le **città in piedi**, le **navi atterrate**, i
**colpi non spesi**.

## Cosa si può toccare

**Sì** a tutto quello che sta sotto `src/Sagas/`: la saga, il suo stato, il processo, le
registrazioni.

**No** agli aggregati, ai guasti e alla curva di difficoltà: sono il campo di gioco, uguale per
tutti.

Un processo vive per **una nave**, non per la città. Non sa cosa stanno facendo gli altri, non sa
quanti colpi restano, non sa quali cannoni sono liberi. Se ti serve saperlo, ricavalo dagli eventi.
