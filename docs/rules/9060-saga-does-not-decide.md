# R-9060: Una saga non decide, e non ospita dominio

- Status: enforced
- Source: [ADR-9020](../adr/9020-saga-owns-the-coordination.md), [ADR-9070](../adr/9070-the-owner-decides.md)

## Rule

Due cose, e sono diverse.

**Una saga non decide.** Riceve eventi e manda comandi. Non contiene `if`: carica lo stato, scarta le
riconsegne, chiede a un processo cosa fare, e fa quello. Il processo — per l'intercettazione è
`InterceptionProcess` — riceve stato ed evento e restituisce una reazione: i comandi da impartire e
se chiudere. Quella classe non tocca né il bus né la persistenza.

Restano legittimi nella saga: lo `switch` sull'esito (`Won`, `Lost`, altrimenti prosegui) e il
`foreach` sugli ordini da spedire. Non sono decisioni, sono il ciclo di vita.

**Il progetto delle saghe non ospita dominio.** Nessuna costante di gioco, nessun conto su colpi,
stazze o tempi, nessuna scelta di risorse. Quelle stanno nell'aggregato che possiede la risorsa.
Quello che vive in `src/Sagas/` è la condotta del processo: insistere quando manca un evento,
riparare, compensare, e chiudere solo a conto saldato.

La differenza si vede da una domanda sola: **se cambio questa riga, cambia il gioco o cambia il modo
di condurlo?** Se cambia il gioco, la riga è nel posto sbagliato.

## Why

Una saga sembra il posto naturale dove mettere la logica, perché è l'unico punto che vede la sequenza
intera. Ma è il punto più costoso in cui metterla: per provarla servono un bus, un repository e un
locator, quindi le decisioni finiscono provate solo giocando. Un processo che è una funzione da stato
ed evento a reazione si prova con un test che non accende niente.

E c'è la ragione di lettura: chi apre una saga vuole sapere **quali eventi attraversano il processo**,
non con che aritmetica si sceglie una risorsa. Due domande diverse, due file diversi.

Il caso di riferimento è la difesa planetaria. La prima versione aveva dentro il progetto delle saghe
il conto della potenza di fuoco, il budget di tempo residuo e la scelta della forza. Spostata la
decisione sulla Terra — che è l'unica a vedere la risorsa — è rimasta la cosa che conta: un processo
che sopravvive a ordini persi e pezzi rotti, e che sa quando ha finito davvero.

## Enforcement

Revisione: in una saga la parola `if` è un difetto. Il modo di non averne è scorrere invece di
interrogare — zero o un ordine è un `foreach`, zero o una saga da far avanzare è un `foreach` — e
lasciare ogni ramo al processo.

Nel progetto `src/Sagas/`, un riferimento a `Armory`, a `Ships` o a qualunque numero che descriva il
mondo è un difetto. L'unica cosa che il processo può leggere da `Contracts/World/` sono gli
identificatori fissi.
