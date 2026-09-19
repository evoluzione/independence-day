# ADR-9020: La saga e' l'unico coordinatore della difesa

- Status: accepted
- Date: 2026-09-15

## Context

Fra l'avvistamento di una flotta e la sua distruzione ci sono quattro o cinque passi, ognuno con un
esito che decide quale sia il passo dopo. Qualcuno deve tenere quel filo.

Ci sono due modi conosciuti di farlo. Nella **coreografia** ogni servizio reagisce agli eventi
dell'altro: la Terra vede l'avvistamento e alza gli scudi da sola, vede gli intercettori perduti e
carica il virus da sola. Nell'**orchestrazione** un processo terzo tiene lo stato del giro e manda i
comandi uno alla volta.

La coreografia costa meno codice — non c'è nessuna saga da scrivere — ma la sequenza smette di esistere
in un posto solo: per sapere cosa succede dopo un virus respinto bisogna cercare chi ascolta quel
evento, e la risposta è sparsa fra gli handler. Con un giro che ha sei esiti diversi, la sequenza
diventa qualcosa che nessuno può leggere per intero.

## Decision

**Una saga orchestrata, `PlanetaryDefenseSaga`, è l'unica cosa che decide quale contromisura viene
dopo.** La Terra esegue comandi e racconta esiti; non sa cosa succederà del suo esito. Lo Spazio non
sa nemmeno che esista una difesa: apprende dagli eventi come è finita la flotta.

La saga tiene uno stato esplicito con il gradino corrente, e ogni suo passo pubblica
`DefenseActionTaken`, che è l'unica cosa che la sala operativa vede del coordinamento.

> **Nota 2026-09-19.** Due frasi di questo ADR non descrivono più il codice. Il «gradino corrente» è
> caduto con la scala delle contromisure ([ADR-9030](9030-defense-escalation-ladder.md)), e «tiene
> lo stato del giro» è caduto con [ADR-9070](9070-the-owner-decides.md), portato alle sue
> conseguenze: lo stato dell'intercettazione è vuoto, perché tutto quello che servirebbe ricordare
> lo sa già la Terra. Resta vero il cuore della decisione — **c'è un solo posto che decide quale
> contromisura viene dopo** — e resta vero che quel posto è una saga orchestrata. Cambia solo che
> non ha bisogno di memoria propria per farlo.

## Consequences

- La sequenza completa si legge in un file solo, e i test la percorrono gradino per gradino.
- La saga diventa un punto di passaggio obbligato: se si ferma, la difesa si ferma. Per questo ogni
  comando deve tornare indietro con una risposta e mai con il silenzio — vedi
  [R-9010](../rules/9010-aggregates-do-not-throw.md).
- Lo stato delle saghe è dato da conservare: una saga fallita resta su Mongo, ed è l'unico posto in cui
  si vede una difesa finita male.
- Un processo in più da tenere vivo rispetto alla coreografia.

## Alternatives considered

- **Coreografia** — Nessuna saga, ogni servizio reagisce agli eventi dell'altro. Meno codice, ma la
  sequenza si disperde fra gli handler e i due servizi finiscono per conoscere il flusso l'uno
  dell'altro, che è la dipendenza che il confine doveva togliere.
- **La Terra coordina sé stessa** — La difesa decide da sola la propria escalation. Funziona finché il
  coordinamento riguarda un servizio solo; il giorno in cui serve un terzo attore va riscritto da capo.

## Documents this decision produced

- Rules: [R-9000](../rules/9000-cross-service-communication.md), [R-9010](../rules/9010-aggregates-do-not-throw.md)
- Guidelines: —
