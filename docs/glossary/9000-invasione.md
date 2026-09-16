# GLO-9000: L'invasione

- Status: active

## Battito

**Il resoconto che la Terra manda ogni mezzo secondo, senza che nessuno lo chieda.** Due righe:
`ShipApproaching` per ogni nave ancora in volo, `CannonStillFiring` per ogni cannone rimasto puntato
su una nave che non c'è più.

- **Aliases / Acronyms**: Heartbeat
- **Context**: È l'unico modo di vedere un ordine perso, perché un ordine perso non produce eventi.
  Non passa dagli aggregati: non è un fatto di dominio, è un resoconto.
- **Related**: Ordine perso, Cannone
- **Source**: `src/Earth/Evoluzione.IndependenceDay.Earth.Facade/BackgroundServices/Heartbeat.cs`

## Campagna

**Una partita, dieci livelli.** Comincia al livello uno con le città a pieno organico e finisce in due
modi: superato il decimo livello la Terra ha vinto, esaurite le città ha perso. A parità di vittoria
conta quante città restano in piedi.

- **Aliases / Acronyms**: —
- **Context**: Ricominciare non cancella niente: apre un'ondata nuova con un numero mai usato prima.
- **Related**: Ondata, Livello
- **Source**: `src/Shared/Evoluzione.IndependenceDay.Contracts/World/Invasion.cs`

## Cannone

**L'arma di una città: una per città, cinque in tutto.** Spara a una nave alla volta, un colpo ogni
quattrocento millisecondi, e **continua finché non gli si dice di smettere**. Ha cinque stati: pronto,
in azione, inceppato, a secco, perduto.

- **Aliases / Acronyms**: Cannon
- **Context**: Quale cannone spari lo sceglie la Terra, non chi coordina: è lei a sapere chi è libero.
  Quando la città cade, il cannone cade con lei.
- **Related**: Colpo, Inceppamento, Cessate il fuoco
- **Source**: `src/Earth/Evoluzione.IndependenceDay.Earth.Domain/Entities/EarthDefense.cs`

## Cessate il fuoco

**L'ordine che spegne un cannone e lo restituisce.** È l'azione compensativa del processo, e la sola
che rimetta indietro qualcosa.

- **Aliases / Acronyms**: CeaseFire
- **Context**: Va **confermato**: anche lui può perdersi, e un processo che chiude senza aspettare la
  conferma lascia un cannone a sparare su relitti per il resto della campagna.
- **Related**: Cannone, Colpo su un relitto, Compensazione
- **Source**: `src/Shared/Evoluzione.IndependenceDay.Contracts/Commands/CeaseFire.cs`

## Colpo

**L'unità di tutto: centodieci per città, cinquecentocinquanta in tutto, e non si ricaricano mai.**
Quello che si spreca al livello due non c'è al livello nove.

- **Aliases / Acronyms**: Round
- **Context**: È la risorsa che decide quanto si va avanti, ma non è quella che scarseggia per prima:
  quella è la disponibilità dei cannoni.
- **Related**: Cannone, Colpo su un relitto
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

**Ogni venti grilletti un cannone si blocca.** Il colpo non parte, non consuma munizioni, e il
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

## Livello

**La difficoltà dell'ondata in corso, e il punteggio.** Sale di uno a ogni ondata superata — più navi
e navi più pesanti — fino al decimo, che è il traguardo.

- **Aliases / Acronyms**: Level
- **Context**: Da non confondere con l'ondata, che è un progressivo di sempre e non si azzera.
- **Related**: Ondata, Campagna
- **Source**: `src/Space/Evoluzione.IndependenceDay.Space.Domain/Services/WaveDifficulty.cs`

## Nave

**Un attacco: una nave sola, di una delle tre stazze.** È l'aggregato dello Spazio, e ha tre stati:
in avvicinamento, abbattuta, atterrata.

- **Aliases / Acronyms**: Ship, AlienShip
- **Context**: Quanti colpi servano ad abbatterla lo sa solo la Terra. Chi coordina lo scopre perché
  la nave cade.
- **Related**: Stazza, Tempo di avvicinamento
- **Source**: `src/Space/Evoluzione.IndependenceDay.Space.Domain/Entities/AlienShip.cs`

## Ondata

**Le navi di un livello, dal primo lancio all'ultimo esito.** Finisce quando nessuna è più in
avvicinamento; da lì si passa alla successiva con un pulsante.

- **Aliases / Acronyms**: Wave
- **Context**: Il numero è un progressivo di sempre e non si ripete mai, nemmeno fra due campagne:
  è la chiave con cui navi e diario si filtrano.
- **Related**: Livello, Campagna
- **Source**: `src/Space/Evoluzione.IndependenceDay.Space.Domain/Entities/Invasion.cs`

## Collegamento

**La radio fra chi coordina e la Terra.** Non è affidabile: un ordine su venticinque si perde per strada.

- **Aliases / Acronyms**: Radio, RadioLink
- **Context**: Sta al bordo del servizio, non nel dominio. Un aggregato che ignorasse un comando
  valido sarebbe una rete che finge; quelli che si perdono non arrivano fino a lui.
- **Related**: Ordine perso, Battito
- **Source**: `src/Earth/Evoluzione.IndependenceDay.Earth.Facade/Messaging/RadioLink.cs`

## Ordine perso

**Un ordine su venticinque non arriva alla Terra.** Si ferma sul collegamento, prima di qualunque
aggregato: non un errore, non un rifiuto, silenzio.

- **Aliases / Acronyms**: OrderLost
- **Context**: È deterministico, e due di fila non si perdono mai: riprovare basta sempre. Non
  esiste un evento che lo racconti, e non è una dimenticanza: se l'ordine non è arrivato, sulla
  Terra non è successo niente.
- **Related**: Battito, Collegamento
- **Source**: `src/Shared/Evoluzione.IndependenceDay.Contracts/World/Radio.cs`

## Stazza

**Quanto è dura una nave**: caccia, incrociatore, corazzata. Uno, tre o sei colpi **a segno** per
abbatterla — e un colpo su tre manca il bersaglio, quindi ne servono di più.
Il danno se tocca terra è lo stesso per tutte: la città non c'è più.

- **Aliases / Acronyms**: ShipClass
- **Context**: Le più pesanti partono per prime, così chi arriva dopo trova i cannoni già impegnati.
- **Related**: Nave, Integrità
- **Source**: `src/Shared/Evoluzione.IndependenceDay.Contracts/World/Ships.cs`

## Tempo di avvicinamento

**Quanto ha la difesa per abbattere una nave prima che tocchi terra.** Otto secondi, configurabili.

- **Aliases / Acronyms**: ApproachSeconds
- **Context**: Otto secondi sono trentadue grilletti: il tempo non è mai la cosa che manca. Il cronometro sta
  sulla Terra, perché è lei a sapere se la nave è ancora viva.
- **Related**: Nave, Cannone
- **Source**: `src/Earth/Evoluzione.IndependenceDay.Earth.Facade/BackgroundServices/ApproachDeadline.cs`
