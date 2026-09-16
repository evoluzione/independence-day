# R-0300: Un effetto esce sul bus solo dopo che l'evento e' durevole

- Status: enforced
- Source: [ADR-0340](../adr/0340-distributed-transactions.md), [ADR-0320](../adr/0320-event-sourcing.md)

## Rule

Un messaggio diretto a un altro servizio si pubblica **dalla proiezione**, cioè dopo che l'evento di
dominio è stato scritto sull'event store. Non si pubblica dentro un command handler, prima o durante
la `SaveAsync`.

## Why

Il catalogo prescrive qui una tabella di outbox. In un sistema event-sourced quella tabella esiste già
e si chiama event store: la sottoscrizione riparte da una posizione salvata, quindi un processo che
muore fra la scrittura e la pubblicazione ripubblica al riavvio invece di perdere il messaggio.

Pubblicare dal command handler perde esattamente quella garanzia: se il processo cade dopo l'invio e
prima della scrittura, il bus ha annunciato qualcosa che non è mai accaduto, e nessuna riesecuzione lo
smentirà.

## Enforcement

Revisione. I publisher verso il bus sono `DomainEventHandlerAsync` in `*.ReadModel/EventHandlers`, e
sono gli unici punti in cui compare `IEventBus`: un `IEventBus` iniettato in un command handler è da
fermare.

<!-- Riscritto a mano rispetto al testo generato da specframe, che prescrive una tabella di outbox:
     qui l'event store fa quel lavoro, e una tabella in piu' non aggiungerebbe nessuna garanzia. -->
