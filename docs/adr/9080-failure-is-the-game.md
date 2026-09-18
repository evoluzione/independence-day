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
guardando l'evento, ed è il caso facile. Il caso interessante per una saga è l'altro: un cannone che
smette di rispondere da solo, senza che nessuno abbia sbagliato niente.

> **Amendment 2026-09-18.** Il gioco aveva anche un quarto guasto, l'ordine perso sul collegamento:
> uno su venticinque, mai arrivato, senza nemmeno un rifiuto — l'unico modo di accorgersene era il
> battito. È stato tolto passando a un'unica ondata da cinque problemi distinti: il collegamento
> inaffidabile duplicava la lezione del primo problema (la contesa sui cannoni, non un ordine
> perduto) senza aggiungerne una nuova, e allargava la superficie del kata oltre le cinque mosse che
> doveva insegnare. **Restano tre guasti**, tutti dentro il dominio; il battito resta obbligatorio,
> ma la sua ragione ora è la contesa, non il silenzio di un ordine.

## Decision

**Tre guasti, tutti deterministici, tutti scritti in chiaro in `Contracts/World/Armory.cs`:**

| | |
| --- | --- |
| **Il bersaglio mancato** | uno su tre. Frequente e banale: il cannone riprova da solo, ma resta occupato |
| **L'inceppamento** | ogni dieci grilletti, e non si sblocca da solo |
| **Il colpo su un relitto** | conseguenza, non causa: un cannone acceso su una nave caduta |

Nessuna probabilità, nessun seme, nessun generatore: contatori. Il pattern sta nella documentazione e
nel codice condiviso: la sfida è gestirlo, non indovinarlo.

## Consequences

- Un processo che si accorge di un cannone fermo si può scrivere, provare e correggere. Senza
  determinismo sarebbe un lavoro a tentoni.
- Il battito resta necessario: è l'unico modo di scoprire che una nave è rimasta scoperta perché
  tutti i cannoni erano impegnati quando è arrivata.
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
