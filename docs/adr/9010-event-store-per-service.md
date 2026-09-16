# ADR-9010: Un event store per servizio

- Status: accepted
- Date: 2026-09-15

## Context

Spazio e Terra sono entrambi event-sourced. La scelta più economica sarebbe una sola istanza di
KurrentDB per tutti e due: un container in meno, una configurazione in meno, e gli stream sono già
separati per aggregato.

Non regge, e il modo in cui non regge è la parte interessante. Ogni servizio si sottoscrive allo
**stream globale** della propria istanza, perché è così che Muflone alimenta le proiezioni. Su
un'istanza condivisa quella sottoscrizione rilegge anche gli eventi dell'altro servizio: tipi che non
conosce, che non sa deserializzare, e che ripubblicherebbe sul proprio bus se per caso li conoscesse.

Il difetto osservato in fase di costruzione non si presentava come un errore di configurazione ma come
una riga di log per ogni evento altrui — e, nel caso peggiore, come eventi doppi sul bus il giorno in
cui un servizio arrivasse a referenziare i messaggi dell'altro.

## Decision

**Due istanze di KurrentDB, `eventstore-space` e `eventstore-earth`.** Ogni servizio scrive e legge
solo la propria, e la sua sottoscrizione globale vede solo eventi che gli appartengono.

Vale anche per Mongo, che ospita un database per servizio (`space-readmodel`, `earth-readmodel`,
`sagas`) sulla stessa istanza: lì basta la separazione logica, perché nessuno legge per scansione
globale.

## Consequences

- Nessun evento estraneo raggiunge una sottoscrizione. I log dei servizi contengono solo fatti propri,
  e un errore di deserializzazione torna a voler dire qualcosa.
- Un container in più, e su Apple Silicon due immagini x64 in emulazione invece di una.
- Non esiste una query che attraversa i due libri mastri. Una correlazione fra i due mondi si fa sugli
  eventi di integrazione, non sugli stream.

## Alternatives considered

- **Una sola istanza condivisa** — Un container in meno, e nell'immediato funziona: gli eventi altrui
  vengono scartati perché il tipo non si risolve. Ma il sistema regge per una coincidenza — che i due
  servizi non si conoscano — e non per una decisione.
- **Una istanza sola con sottoscrizioni per categoria** — Eviterebbe il problema filtrando a monte, ma
  legherebbe la correttezza a una convenzione sui nomi degli stream, cioè a qualcosa che nessuno
  verifica al momento della scrittura.

## Documents this decision produced

- Rules: —
- Guidelines: —
