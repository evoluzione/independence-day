# ADR-9040: L'esito della battaglia deciso fuori dall'aggregato

- Status: superseded
- Superseded by: [ADR-9060](9060-deterministic-combat.md)
- Date: 2026-09-15

## Superata da ADR-9060

Il combattimento non ha piu' un esito incerto, quindi non c'e' piu' un dado da tirare ne' un posto
dove tirarlo. Il ragionamento resta valido — un aggregato event-sourced non puo' contenere il caso —
ma non ha piu' un caso da contenere.

## Context

Uno scontro ha un esito incerto: gli intercettori possono fermare la flotta o essere abbattuti, il
virus può attecchire o essere respinto. Da qualche parte quel dado va tirato.

Il posto che sembra naturale è l'aggregato, che è dove stanno le regole. Ma un aggregato event-sourced
viene **reidratato rileggendo i propri eventi**: se tirasse un dado dentro una regola, la rilettura
darebbe un risultato diverso dalla prima volta e lo stato ricostruito non sarebbe quello che è stato
scritto. Lo stesso vale, in modo più sottile, per una riconsegna del comando: il broker consegna almeno
una volta, e la seconda esecuzione dello stesso comando produrrebbe una battaglia diversa dalla prima.

## Decision

**L'esito lo decide `IBattleOutcome`, un servizio di dominio iniettato nel command handler.** Il
handler chiede l'esito, lo passa all'aggregato come un dato, e l'aggregato lo registra nell'evento. Da
quel momento in poi non è più una probabilità: è storia.

L'implementazione di produzione non usa `Random`. Semina il tiro sugli identificativi in gioco — la
flotta, la nave madre — con un hash stabile fra processi, così lo stesso scontro dà sempre lo stesso
esito su qualunque istanza e a qualunque riconsegna.

## Consequences

- L'aggregato resta puro: date le stesse premesse produce sempre gli stessi eventi, e i test lo
  verificano senza artifici.
- Entrambi i rami — vittoria e sconfitta — sono provabili sostituendo il servizio, ed è quello che
  fanno i test di `Earth.Domain.Tests`.
- La formula che calcola lo squadrone vive nell'aggregato ed è esposta come statica, perché al handler
  serve la stessa per chiedere le probabilità di quello squadrone: due copie si scollerebbero al primo
  ritocco e il tiro verrebbe fatto su uno squadrone diverso da quello che parte.
- Il tiro seminato sugli id non è casuale in senso statistico: è ripetibile, ed è quello che serve qui.

## Alternatives considered

- **`Random` dentro l'aggregato** — Il posto più naturale a leggerlo, e il più sbagliato: la
  reidratazione ricostruirebbe uno stato diverso da quello scritto, in silenzio.
- **`Random` nel command handler** — Risolve la reidratazione ma non la riconsegna: lo stesso comando
  consegnato due volte darebbe due esiti, e il read model racconterebbe una battaglia mai avvenuta.

## Documents this decision produced

- Rules: —
- Guidelines: —
