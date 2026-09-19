# R-9060: Una saga conduce il processo, non contiene il dominio

- Status: enforced
- Source: [ADR-9020](../adr/9020-saga-owns-the-coordination.md), [ADR-9070](../adr/9070-the-owner-decides.md)

## Rule

**Il progetto delle saghe non ospita dominio.** Nessuna costante di gioco, nessun conto su colpi,
stazze o tempi, nessuna scelta di risorse. Quelle stanno nell'aggregato che possiede la risorsa.

Quello che vive in `src/Sagas/` è la condotta di un processo distribuito: insistere quando manca un
evento, riparare, compensare, e chiudere solo a conto saldato. Sono decisioni, e stanno nella saga —
ma sono decisioni *sul processo*, non *sul mondo*.

La differenza si vede da una domanda sola: **se cambio questa riga, cambia il gioco o cambia il modo
di condurlo?** Se cambia il gioco, la riga è nel posto sbagliato.

Una saga è **due file**: lo stato e la saga. Lo stato è quello che si ricorda fra un evento e
l'altro — ma solo di quello che **nessun altro sa già**. Se il proprietario della risorsa lo sa, non
va ricordato qui: una copia in ritardo di una verità che vive altrove è la stessa violazione di
prima, scritta come campo invece che come `if`. Quando il proprietario sa tutto, lo stato resta
vuoto, e va bene così. La saga è un handler per evento, ognuno corto abbastanza da leggersi tutto
insieme, più il giro comune — carica, scarta le riconsegne, decide, salva, spedisce.

## Why

La versione precedente di questa regola diceva l'opposto: niente `if` nella saga, e la decisione in
una classe a parte che restituiva una reazione. L'argomento era la testabilità — una funzione pura
da stato ed evento si prova senza accendere niente.

L'argomento non regge. I sostituti per provare una saga ci sono già e sono tre file da venti righe
(`RecordingServiceBus`, `InMemorySagaRepository`, `NoStateLocator`): con quelli si prova la saga
**vera**, inclusi lo scarto delle riconsegne e la chiusura, che con la funzione pura restavano fuori.
Due dei test del processo passavano su sequenze che nel sistema vero non capitano, e nessuno se ne
era accorto perché non toccavano mai la saga.

Quello che resta è il costo: tre file — saga, processo, reazione — per un processo che ne chiede due,
e la sequenza spezzata in due punti da tenere allineati a mano. Per chi apre il repository per la
prima volta e ha un'ora, è una tassa senza contropartita.

La separazione resta giusta quando la decisione è **di dominio** e si può calcolare su dati inerti.
Ma quella, per [ADR-9070](../adr/9070-the-owner-decides.md), non deve stare qui: sta sulla Terra.
Tolta quella, non resta niente da separare.

## Enforcement

Revisione: nel progetto `src/Sagas/`, un riferimento ad `Armory`, a `Ships` o a qualunque numero che
descriva il mondo è un difetto. L'unica cosa che una saga può leggere da `Contracts/World/` sono gli
identificatori fissi.

Un handler che non si legge in una schermata sta facendo un lavoro che non è suo: quasi sempre è un
conto di dominio, e va chiesto a chi possiede la risorsa.
