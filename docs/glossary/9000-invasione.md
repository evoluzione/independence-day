# GLO-9000: L'invasione

- Status: active

## Battito

**Il resoconto che la Terra manda ogni mezzo secondo, senza che nessuno lo chieda.** Una riga,
`ShipApproaching`, per ogni nave ancora in volo — con quanti cannoni le sono addosso in quel momento.

- **Aliases / Acronyms**: Heartbeat
- **Context**: È l'unico modo di vedere che una nave è rimasta scoperta perché tutti i cannoni erano
  impegnati quando è arrivata. Non passa dagli aggregati: non è un fatto di dominio, è un resoconto.
- **Related**: Cannone, Intercettazione
- **Source**: `src/Earth/Evoluzione.IndependenceDay.Earth.Facade/BackgroundServices/Heartbeat.cs`

## Campagna

**Una partita, un'ondata.** Comincia con le città a pieno organico e finisce in due modi: respinta
l'ondata la Terra ha vinto, esaurite le città ha perso. A parità di vittoria conta quante città
restano in piedi.

- **Aliases / Acronyms**: —
- **Context**: Ricominciare non lascia niente della partita precedente: le città tornano a pieno
  organico e riparte una campagna nuova.
- **Related**: Ondata
- **Source**: `src/Shared/Evoluzione.IndependenceDay.Contracts/World/Invasion.cs`

## Cannone

**L'arma di una città: una per città, cinque in tutto.** Spara a una nave alla volta, un colpo ogni
trecentocinquanta millisecondi, e **continua finché non gli si dice di smettere**. Ha sei stati:
pronto, in azione, inceppato, a secco, in rifornimento, perduto.

- **Aliases / Acronyms**: Cannon
- **Context**: Quale cannone spari lo sceglie la Terra, non chi coordina: è lei a sapere chi è libero.
  Quando la città cade, il cannone cade con lei.
- **Related**: Colpo, Inceppamento, Rifornimento, Cessate il fuoco
- **Source**: `src/Earth/Evoluzione.IndependenceDay.Earth.Domain/Entities/EarthDefense.cs`

## Cessate il fuoco

**L'ordine che spegne un cannone e lo restituisce.** È l'azione compensativa del processo, e la sola
che rimetta indietro qualcosa.

- **Aliases / Acronyms**: CeaseFire
- **Context**: Va **confermato**: un processo che chiude senza aspettare la conferma lascia un
  cannone a sparare su relitti per il resto della campagna.
- **Related**: Cannone, Colpo su un relitto, Intercettazione
- **Source**: `src/Shared/Evoluzione.IndependenceDay.Contracts/Commands/CeaseFire.cs`

## Colpo

**L'unità di tutto: quaranta per città all'inizio, duecento in tutto.** Non si ricarica da solo, ma
un cannone a secco si rifornisce: non è un budget fisso, è un punto di partenza.

- **Aliases / Acronyms**: Round
- **Context**: È la risorsa che decide quanto si va avanti, ma non è quella che scarseggia per prima:
  quella è la disponibilità dei cannoni.
- **Related**: Cannone, Rifornimento, Colpo su un relitto
- **Source**: `src/Shared/Evoluzione.IndependenceDay.Contracts/World/Armory.cs`

## Bersaglio mancato

**Un colpo su tre manca il bersaglio.** La munizione se ne va e la nave regge.

- **Aliases / Acronyms**: ShotMissed
- **Context**: Non è un guasto e non c'è niente da fare: il fuoco è aperto, quindi il cannone
  ricarica e riprova da solo. Il prezzo non è la munizione, è il tempo in cui quel cannone resta
  occupato — da non confondere con il **colpo su un relitto**, che è un cessate il fuoco dimenticato.
- **Related**: Colpo, Colpo su un relitto
- **Source**: `src/Shared/Evoluzione.IndependenceDay.Contracts/World/Armory.cs`

## Colpo su un relitto

**Un colpo sparato a una nave che non c'è più.** Non è un guasto della Terra: è un cessate il fuoco
che non è arrivato.

- **Aliases / Acronyms**: ShotWasted
- **Context**: Costa due volte — la munizione, e il cannone che non è disponibile per la nave dopo.
  È l'unico difetto del processo che non produce nessun errore.
- **Related**: Cessate il fuoco, Battito
- **Source**: `src/Earth/Evoluzione.IndependenceDay.Earth.Messages/Events/EarthShotWasted.cs`

## Inceppamento

**Ogni dieci grilletti un cannone si blocca.** Il colpo non parte, non consuma munizioni, e il
cannone si ferma lasciando la nave senza nessuno addosso. Il conto è sui grilletti premuti, colpi a
vuoto compresi.

- **Aliases / Acronyms**: Jam
- **Context**: Non si sblocca da solo. La riparazione costa tre colpi e **non** riapre il fuoco.
- **Related**: Cannone, Riparazione
- **Source**: `src/Shared/Evoluzione.IndependenceDay.Contracts/World/Armory.cs`

## Integrità

**La vita di una città: cento, oppure zero.** Una nave che tocca terra la rade al suolo, qualunque
sia la stazza.

- **Aliases / Acronyms**: Integrity
- **Context**: Le città in piedi sono le vite della Terra, e ognuna porta con sé un cannone: perderne
  una vale un quinto della potenza di fuoco per tutto il resto della campagna.
- **Related**: Cannone, Campagna
- **Source**: `src/Earth/Evoluzione.IndependenceDay.Earth.Domain/Entities/EarthDefense.cs`

## Intercettazione

**Il processo che porta giù una nave e restituisce i cannoni che ha impegnato.** Uno per nave. Non
finisce quando la nave cade: finisce quando il conto con la Terra è chiuso.

- **Aliases / Acronyms**: Interception, saga
- **Context**: Non vede le altre intercettazioni, non vede i colpi rimasti, non sa quanti ne servano.
  Quello che sa lo ha sentito dire.
- **Related**: Cessate il fuoco, Battito
- **Source**: `src/Sagas/Evoluzione.IndependenceDay.Sagas/ShipInterception/ShipInterceptionSaga.cs`

## Nave

**Un attacco: una nave sola, di una delle tre stazze.** È l'aggregato dello Spazio, e ha tre stati:
in avvicinamento, abbattuta, atterrata.

- **Aliases / Acronyms**: Ship, AlienShip
- **Context**: Quanti colpi servano ad abbatterla lo sa solo la Terra. Chi coordina lo scopre perché
  la nave cade.
- **Related**: Stazza, Tempo di avvicinamento
- **Source**: `src/Space/Evoluzione.IndependenceDay.Space.Domain/Entities/AlienShip.cs`

## Ondata

**Tutta la campagna: trentasei navi, dal primo lancio all'ultimo esito.** Finisce quando nessuna è
più in avvicinamento.

- **Aliases / Acronyms**: Wave
- **Context**: Il numero è un progressivo di sempre e non si ripete mai, nemmeno fra due campagne:
  è la chiave con cui navi e diario si filtrano.
- **Related**: Campagna
- **Source**: `src/Space/Evoluzione.IndependenceDay.Space.Domain/Entities/Invasion.cs`

## Rifornimento

**La consegna che ricarica un cannone a secco.** Illimitata — non c'è un tetto di consegne per
campagna — ma non immediata: il convoglio ci mette tre secondi a portare venti colpi.

- **Aliases / Acronyms**: Resupply
- **Context**: Come la riparazione, non riapre il fuoco: rimette il cannone disponibile, e fermo.
  Un cannone a secco si libera subito — non resta assegnato alla sua nave, a differenza di un
  cannone inceppato — quindi chi coordina non deve restituirlo, solo chiederne un altro.
- **Related**: Cannone, Colpo, Inceppamento
- **Source**: `src/Earth/Evoluzione.IndependenceDay.Earth.Facade/BackgroundServices/SupplyConvoy.cs`

## Stazza

**Quanto è dura una nave**: caccia, incrociatore, corazzata. Uno, quattro o nove colpi **a segno** per
abbatterla — e un colpo su tre manca il bersaglio, quindi ne servono di più.
Il danno se tocca terra è lo stesso per tutte: la città non c'è più.

- **Aliases / Acronyms**: ShipClass
- **Context**: L'ondata è a blocchi, non tutte le corazzate in testa: le più pesanti aprono ogni
  blocco, così chi arriva dopo trova i cannoni già impegnati senza che le prime inceppino tutti e
  cinque i cannoni in una volta sola.
- **Related**: Nave, Integrità
- **Source**: `src/Shared/Evoluzione.IndependenceDay.Contracts/World/Ships.cs`

## Tempo di avvicinamento

**Quanto ha la difesa per abbattere una nave prima che tocchi terra.** Otto secondi, configurabili.

- **Aliases / Acronyms**: ApproachSeconds
- **Context**: Otto secondi sono una ventina di grilletti: per una corazzata, che ne chiede nove a
  segno, il tempo è appena sufficiente se il cannone non si inceppa mai. Il cronometro sta sulla
  Terra, perché è lei a sapere se la nave è ancora viva.
- **Related**: Nave, Cannone
- **Source**: `src/Earth/Evoluzione.IndependenceDay.Earth.Facade/BackgroundServices/ApproachDeadline.cs`
