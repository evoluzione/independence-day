# ADR-9070: Chi possiede la risorsa prende la decisione

- Status: accepted
- Date: 2026-09-16
- Amends: [ADR-9020](9020-saga-owns-the-coordination.md)

## Context

La prima versione di questo kata chiedeva a chi scriveva la saga di scegliere **quale forza mandare e
quanta**. Per farlo doveva conoscere la potenza di fuoco di ogni unità, i tempi di marcia, la finestra
di avvicinamento e un margine per il giro completo del messaggio. Tutta quella conoscenza finiva in
una classe dentro `src/Sagas/`.

Funzionava, e il risultato era una cosa diversa da quella che l'esercizio dichiarava. La saga vera —
gli handler, il trasporto, la chiusura — si scriveva in dieci minuti; le due ore restanti se le
mangiava l'aritmetica. Si poteva vincere la campagna senza aver mai affrontato un timeout, una
compensazione o un evento che non arriva. E la stessa conoscenza viveva in due posti: nell'aggregato
che eseguiva e nella saga che sceglieva, d'accordo per costruzione ma non per meccanismo.

C'era anche un difetto più profondo. La saga sceglieva la forza **senza vedere la riserva**: la
riserva è condivisa fra cinque città e una saga vive per una minaccia sola. Stava quindi decidendo
sull'allocazione di una risorsa di cui non poteva conoscere lo stato, e l'unico modo di scoprirlo era
sentirsi rifiutare un ordine.

## Decision

**La decisione sta dove sta la risorsa.** La Terra possiede cannoni, munizioni e combattimento, quindi
è la Terra a scegliere quale cannone spara, quanti colpi servono e quando un pezzo si rompe. Chi
coordina non lo sa e non deve saperlo: chiede che una nave venga presa di mira, e si sente rispondere
con quale città — o che non c'è nessun cannone libero.

Il progetto `src/Sagas/` non contiene **nessuna** regola di gioco: nessuna costante, nessun conto,
nessuna tabella. Contiene la condotta di un processo distribuito, che è una cosa diversa e ha i suoi
problemi:

- un ordine può non arrivare, e non produrre nessun evento;
- un passo può guastarsi a metà;
- un passo già fatto può dover essere **disfatto**, e la disfatta può a sua volta perdersi;
- il processo finisce quando il conto è chiuso, non quando l'obiettivo è raggiunto.

Quello che resta nella saga è la macchina a stati di questi quattro problemi: un handler per evento,
e due file in tutto — la saga e il suo stato.

## Consequences

- La saga si legge come un elenco di cose che possono andare storte, che è quello che si voleva insegnare.
- Nessuna conoscenza duplicata: quanti colpi regga una corazzata è scritto in un posto solo, e chi
  coordina lo scopre perché la nave cade.
- La Terra è diventata più grossa: sceglie il cannone, tiene il contatore dei guasti, e la scelta
  «il libero con più colpi» è una regola di dominio in più da mantenere.
- L'esercizio è più difficile da *capire* e più facile da *scrivere*: il codice da produrre è poco,
  ma va messo nei punti giusti.
- Un partecipante che cerca la leva strategica non la trova. È voluto, e va detto nel README —
  altrimenti la cerca per mezz'ora.

## Alternatives considered

- **Lasciare la scelta della forza a chi coordina, spostando solo l'aritmetica in un servizio di
  dominio condiviso** — Avrebbe tolto i conti dalla saga lasciandole la decisione. Ma la decisione
  *è* il problema: senza vedere la risorsa condivisa, resta una scelta presa alla cieca.
- **Dare a chi coordina una vista sulla riserva** — Renderebbe la scelta sensata, al prezzo di far
  leggere a un servizio il read model di un altro. È esattamente quello che
  [R-0090](../rules/0090-no-cross-service-db.md) vieta, e per buone ragioni.

## Documents this decision produced

- Rules: [R-9060](../rules/9060-saga-does-not-decide.md), [R-9070](../rules/9070-compensation-is-confirmed.md)
- Guidelines: —
