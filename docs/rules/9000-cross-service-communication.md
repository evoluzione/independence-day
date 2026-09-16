# R-9000: L'unica interfaccia fra due servizi e' il bus

- Status: enforced
- Source: [ADR-9000](../adr/9000-service-boundaries.md), [ADR-9020](../adr/9020-saga-owns-the-coordination.md)

## Rule

[R-0090](0090-no-cross-service-db.md) vieta di leggere il database di un altro servizio e lascia aperta
la sua "interfaccia pubblicata". Qui quell'interfaccia è una sola: **il bus**.

Un servizio non chiama un altro servizio in HTTP. L'unica cosa che attraversa un confine è un messaggio,
e il suo tipo vive in `Evoluzione.IndependenceDay.Contracts`.

Vale anche per le letture: se alla Terra serve sapere che stazza ha una nave, se lo costruisce
proiettando gli eventi che vede passare — è quello che fa la collection `Threat` — non interrogando lo
Spazio.

## Why

Una chiamata diretta non si presenta come un problema finché l'altro servizio risponde. Il giorno in cui
è lento o giù, il primo si ferma con lui, e quello che sembrava un sistema a tre pezzi si scopre essere
stato uno solo dall'inizio.

Il bus toglie anche la domanda "chi chiama chi": nessuno dei tre servizi conosce l'esistenza degli
altri due, e il grafo delle dipendenze resta piatto qualunque cosa si aggiunga.

## Enforcement

I progetti non si referenziano: `Space` e `Earth` non si vedono, e nessuno dei due apre il Mongo
dell'altro. Un `HttpClient` verso un servizio interno è da fermare in revisione.
