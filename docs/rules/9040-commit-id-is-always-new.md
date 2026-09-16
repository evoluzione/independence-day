# R-9040: Il commitId di un comando e' sempre nuovo, il correlationId viaggia a parte

- Status: enforced
- Source: [ADR-9020](../adr/9020-saga-owns-the-coordination.md)

## Rule

Un comando si costruisce con un `commitId` nuovo (`Guid.NewGuid()`) e porta il correlationId come
proprietà esplicita. Le classi base `DefenseCommand` e `SpaceCommand` lo fanno già: i comandi nuovi
derivano da quelle.

## Why

Il `commitId` diventa l'identità dell'append su EventStore, che lo usa per riconoscere le riconsegne.

La saga manda quattro comandi diversi — scudi, intercettori, virus, atomica — e vanno tutti sullo
**stesso** aggregato città, sotto lo stesso correlationId. Se il correlationId facesse anche da
commitId, dal secondo comando in poi l'event store considererebbe l'append una riconsegna e lo
scarterebbe.

Silenziosamente: nessun errore, nessun evento, e la difesa ferma al primo gradino.

## Enforcement

Revisione, e le due classi base che rendono la cosa automatica. Un comando che chiama
`Command(aggregateId, correlationId, who)` direttamente è da fermare.
