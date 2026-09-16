# Le regole

## I cannoni

La Terra ha **cinque città, un cannone ciascuna**. Non c'è una riserva da distribuire: il cannone è
lì, è di quella città, e cade con lei.

| | |
| --- | --- |
| Colpi in dotazione | **110 per città**, 550 in tutto |
| Ricarica | un colpo ogni **250 ms** |
| Bersagli | **uno alla volta**: un cannone spara a una nave sola |
| Raggio | tutte le città sparano a **tutte** le navi, non solo a quella sopra di loro |

Le munizioni **non si ricaricano mai** — né fra un'ondata e l'altra, né durante. I 550 colpi sono
tutti quelli che ci sono da qui alla fine della campagna.

**Quale cannone spari non lo decidi tu.** Chiedi di aprire il fuoco su una nave, e la Terra sceglie:
prende quello libero con più colpi. È lei a sapere chi è impegnato, chi è rotto e chi è a secco.

## Il fuoco continuato

Un ordine di apertura non è un colpo: **accende** un cannone. Da quel momento spara da solo, ogni
250 ms, finché non gli si dice di smettere.

**Non si ferma quando la nave cade.** Continua a sparare su un relitto, e ogni colpo è perso due
volte: la munizione, e il cannone che non c'è quando arriva la nave dopo.

Fermarlo è un ordine a parte — `CeaseFire` — e va **confermato**. Anche lui può perdersi.

**Un colpo su tre manca il bersaglio.** La munizione se ne va e la nave regge. Non c'è niente da
fare e non te lo dice nessuno: il fuoco è aperto, quindi il cannone ricarica e riprova da solo. Il
prezzo vero non è la munizione, è che quel cannone resta occupato più a lungo.

## Le navi

Ogni attacco è **una nave sola**. Tre stazze, e quanti colpi servano ad abbatterle **non lo dice
nessun evento**: lo sa solo la Terra. Lo si scopre perché la nave cade.

| Stazza | Colpi per abbatterla |
| --- | ---: |
| 🛸 Caccia | 1 |
| 🛰️ Incrociatore | 3 |
| 🚀 Corazzata | 6 |

**Una nave che tocca terra rade al suolo la città.** Qualunque stazza, anche un caccia al primo
livello. Non c'è un'integrità da erodere: o la fermi, o quella città non c'è più — e con lei se ne
vanno il cannone e i colpi che gli restavano, cioè **un quinto della potenza di fuoco** per tutto il
resto della campagna.

Non difendersi non è un'opzione con un costo: è la sconfitta alla prima ondata.

## Il tempo

Dalla presa in carico di una nave hai **8 secondi**. Poi tocca terra.

Otto secondi sono trentadue grilletti, e una corazzata ne chiede nove fra colpi a segno e colpi a
vuoto: **un cannone solo basta**, e non serve mandarne due. Quello che scarseggia non è il tempo, sono i cannoni — e un cannone impegnato su una
nave già caduta è un cannone che non c'è.

## I tre guasti

Non sono errori: sono il gioco. Sono **deterministici** — nessun dado, nessuna probabilità nascosta —
e stanno scritti in chiaro in `Contracts/World/Radio.cs` e `Contracts/World/Armory.cs`.

### 1. L'ordine perso — uno su venticinque

Un ordine su venticinque **non arriva alla Terra**. Si perde sul collegamento, prima di raggiungere
qualunque cannone: sulla Terra non succede niente, quindi non c'è niente da raccontare e non torna
indietro nessun evento — nemmeno un rifiuto. Vale per tutti e tre gli ordini.

Non c'è niente da intercettare. L'unico modo di accorgersene è il **battito**.

Non è un aggregato che decide di ignorarti: un comando che arriva a un aggregato produce **sempre**
un evento. Quelli che si perdono non ci arrivano proprio.

Due ordini di fila non si perdono mai: riprovare basta sempre, e non può avvitarsi.

### 2. L'inceppamento — ogni venti colpi

Al ventesimo grilletto un cannone si inceppa: il colpo non parte, non consuma munizioni, e il
cannone si ferma — lasciando la nave che stava affrontando **senza nessuno addosso**.

Il conto è sui grilletti premuti, non sui colpi a segno: valgono anche quelli che hanno mancato il
bersaglio e quelli sparati contro un relitto.

Non si sblocca da solo. Se nessuno manda `RepairCannon` è perso per il resto della campagna. La
riparazione costa **3 colpi** e **non riapre il fuoco**: rimette il cannone disponibile, fermo.

### 3. Il colpo su un relitto

Un cannone lasciato acceso su una nave che non c'è più spara comunque. Non è un guasto della Terra:
è un cessate il fuoco che non è arrivato.

## Il battito

Ogni mezzo secondo la Terra racconta come stanno le cose, senza che nessuno lo chieda. Sono due righe, e
servono a vedere le due cose che **nessun evento** può raccontare, perché sono assenze:

| Battito | Dice | Vuol dire |
| --- | --- | --- |
| `ShipApproaching` | nave viva, quanto manca, **quanti cannoni le sparano** | zero cannoni = l'apertura si è persa |
| `CannonStillFiring` | questo cannone spara a una nave che non c'è più | il cessate il fuoco si è perso |

È l'unico orologio che hai.

## I dieci livelli

Una nave parte ogni **secondo** e punta le città **a turno**. Le più pesanti partono per prime: chi
arriva dopo trova i cannoni già impegnati.

| Livello | Navi | 🛸 | 🛰️ | 🚀 | Colpi necessari | Cumulato |
| ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| 1 | 5 | 5 | 0 | 0 | 5 | 5 |
| 2 | 6 | 5 | 1 | 0 | 8 | 13 |
| 3 | 7 | 5 | 2 | 0 | 11 | 24 |
| 4 | 8 | 5 | 3 | 0 | 14 | 38 |
| 5 | 9 | 5 | 4 | 0 | 17 | 55 |
| 6 | 10 | 4 | 5 | 1 | 25 | 80 |
| 7 | 11 | 3 | 6 | 2 | 33 | 113 |
| 8 | 12 | 2 | 7 | 3 | 41 | 154 |
| 9 | 13 | 1 | 8 | 4 | 49 | 203 |
| 10 | 14 | 0 | 9 | 5 | 57 | **260** |

I colpi in tabella sono quelli **a segno**. Con un colpo su tre mancati, quelli da sparare sono la
metà in più: circa 390 su tutta la campagna.

390 colpi da sparare contro 550 in dotazione, meno quelli che se ne vanno nelle riparazioni. **La
soluzione di riferimento arriva al livello 10 con un centinaio di colpi e cinque città in piedi,
senza lasciar passare nemmeno una nave.** Una che non chiude i cannoni muore al quarto, con nove
colpi su dieci sparati contro relitti.

Il vincolo vero però non sono i colpi: sono i **cannoni liberi**. Al decimo livello le corazzate
partono insieme e se li prendono tutti, e chi arriva dopo aspetta. Un cannone lasciato acceso su un
relitto non è un colpo sprecato: è un posto vuoto in quella fila.

## Come si vince

Superare il livello 10. La campagna è persa quando cade l'ultima città.

A parità di vittoria contano, in quest'ordine: le **città rimaste in piedi**, le **navi atterrate** in
tutta la campagna, i **colpi non spesi**. Sono tutti nel resoconto a fine ondata.

## Cosa si può toccare

**Sì** a tutto quello che sta sotto `src/Sagas/`: la saga, il suo stato, il processo, le registrazioni.

**No** agli aggregati, ai guasti e alla curva di difficoltà: sono il campo di gioco, uguale per tutti.

Un processo vive per **una nave**, non per la città. Non sa cosa stanno facendo gli altri, non sa
quanti colpi restano, non sa quali cannoni sono liberi. Se ti serve saperlo, ricavalo dagli eventi.
