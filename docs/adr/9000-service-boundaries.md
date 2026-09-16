# ADR-9000: Tre servizi e non un monolite modulare

- Status: accepted
- Date: 2026-09-15

## Context

Il dominio ha tre parti con responsabilità nette: lo **Spazio**, che genera gli attacchi; la **Terra**,
che possiede le difese; il **coordinamento**, che guarda gli eventi e decide la reazione. Tutte e tre
possono stare in un solo processo — è quello che fa il repository di riferimento, con moduli separati
dentro un unico host e un bus che li collega comunque.

La tensione è fra il costo del confine e quello che il confine dimostra. Un monolite modulare costa
meno da avviare e da leggere, e il bus ci sarebbe lo stesso. Tre processi costano tre container, tre
configurazioni e una latenza vera fra un comando e il suo esito, ma rendono impossibile la scorciatoia
che in un monolite prima o poi qualcuno prende: chiamare direttamente la facciata dell'altro contesto.

## Decision

**Tre deployable separati: `space-api`, `earth-api`, `defense-saga`.** Non condividono processo, non
condividono base dati, e non si vedono in HTTP. L'unica cosa che attraversa un confine è un messaggio,
e il suo contratto sta in `src/Shared/Evoluzione.IndependenceDay.Contracts`.

Il coordinamento è un processo a sé e non un modulo dentro la Terra, perché non è né lo Spazio né la
Terra: è la cosa che li tiene insieme, e ospitarlo dentro uno dei due lo farebbe sembrare parte di
quello.

## Consequences

- La regola "si parla solo via bus" non è una convenzione da ricordare: è l'unica strada che esiste.
- Ogni esito è asincrono. Non c'è nessun punto del codice in cui si può aspettare una risposta, e la
  saga deve gestire ogni ramo negativo esplicitamente — vedi [ADR-9030](9030-defense-escalation-ladder.md).
- Tre container invece di uno: l'avvio è più lento e i log stanno in tre posti.
- Tornare indietro è caro: i tre host diventerebbero moduli, ma i contratti e gli event store separati
  andrebbero rifusi.

## Alternatives considered

- **Monolite modulare** — Un host solo con i moduli dentro, come il repository di riferimento. Più
  veloce da avviare e da capire, ma il confine fra Spazio e Terra diventa una regola scritta nei
  documenti invece che un fatto del sistema, e la saga si confonde con il modulo che la ospita.
- **Due servizi, saga dentro la Terra** — Meno container, ma il coordinamento finisce a sembrare una
  funzione della difesa, che è esattamente il contrario di quello che è.

## Documents this decision produced

- Rules: [R-9000](../rules/9000-cross-service-communication.md)
- Guidelines: [GL-9000](../guidelines/9000-context-project-layout.md)
