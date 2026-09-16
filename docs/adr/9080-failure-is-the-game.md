# ADR-9080: Il guasto è il gioco, ed è deterministico

- Status: accepted
- Date: 2026-09-16

## Context

Un processo distribuito che funziona sempre non insegna niente. Le cose interessanti — ritentare,
compensare, chiudere il conto — esistono solo perché qualcosa può andare storto, e in un gioco in cui
non va mai storto niente si scrivono handler che sembrano giusti e non lo sono.

Servivano quindi dei guasti. Il modo ovvio è il caso: una probabilità, un generatore, e ogni partita
diversa. Il modo ovvio ha due difetti seri per un kata. Il primo è l'equità: due squadre affronterebbero
partite diverse, e vincere diventerebbe in parte fortuna. Il secondo è peggiore: un guasto casuale non
è riproducibile, quindi un processo sbagliato può passare, e chi lo ha scritto non saprà mai perché
l'altra volta funzionava.

C'è anche la questione di **quale** guasto. Un errore che torna indietro come evento si gestisce
guardando l'evento, ed è il caso facile. Il caso che rompe davvero le saghe è l'altro: il messaggio
che non arriva, che non produce nulla, e su cui non c'è niente da intercettare.

## Decision

**Tre guasti, tutti deterministici, tutti scritti in chiaro in `Contracts/World/Armory.cs`:**

| | |
| --- | --- |
| **L'ordine perso** | uno su tredici, e non produce nessun evento |
| **L'inceppamento** | ogni nove grilletti, e non si sblocca da solo |
| **Il colpo nel vuoto** | conseguenza, non causa: un cannone acceso su una nave caduta |

Uno su tredici e non uno su sette, perché una nave che tocca terra rade al suolo la città: una difesa
condotta bene deve poterle fermare tutte, e con guasti più fitti la perfezione diventava
irraggiungibile. Un gioco che si perde comunque non insegna a giocarlo meglio.

Nessuna probabilità, nessun seme, nessun generatore: contatori. Il conto dei tentativi avanza anche
quando l'ordine si perde, quindi **due ordini di fila non si perdono mai** — riprovare basta sempre, e
non può avvitarsi in un giro infinito.

Il pattern sta nella documentazione e nel codice condiviso: la sfida è gestirlo, non indovinarlo.

L'ordine perso non è silenzioso per omissione. `EarthOrderLost` viene sollevato e proiettato nel
diario, ma **non ha una traduzione di integrazione**: resta dentro la Terra. Chi ha ordinato non
riceve niente, e a schermo resta la traccia di perché quella città era scoperta.

## Consequences

- Un processo che si accorge del silenzio si può scrivere, provare e correggere. Senza determinismo
  sarebbe un lavoro a tentoni.
- Il battito diventa obbligatorio: senza un orologio, un ordine perso è invisibile per sempre.
- Le sequenze non sono comunque identiche fra due partite: l'ordine in cui i comandi arrivano
  all'aggregato dipende dalla coda, quindi **quali** ordini si perdono varia. Il *pattern* è fisso, la
  sua incidenza sulle singole città no. È un compromesso accettato: l'alternativa sarebbe serializzare
  tutto, e sarebbe un altro sistema.
- Un partecipante può leggere `Armory` e prevedere il prossimo inceppamento. Non gli serve a niente —
  non può scegliere il cannone — e sapere come funziona il mondo fa parte del gioco.
- La simulazione della campagna è riproducibile, quindi utilizzabile per tarare il bilanciamento.

## Alternatives considered

- **Probabilità con un seme condiviso** — Riproducibile e più realistica, ma il seme va propagato
  attraverso tre servizi e un broker per restare tale, e il primo riavvio lo perde.
- **Guasti che tornano come evento di errore** — Molto più facili da gestire, e avrebbero tolto
  l'unica cosa che rende la sfida quello che è: l'assenza come esito.
- **Nessun guasto, difficoltà solo dalla curva delle ondate** — Sarebbe un gioco di ottimizzazione, e
  quello lo si era già provato: vedi [ADR-9070](9070-the-owner-decides.md).

## Documents this decision produced

- Rules: [R-9070](../rules/9070-compensation-is-confirmed.md)
- Guidelines: —
