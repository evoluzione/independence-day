# R-9020: Una proiezione registra l'errore e prosegue

- Status: enforced
- Source: [ADR-9010](../adr/9010-event-store-per-service.md)

## Rule

Un handler che aggiorna un read model cattura le proprie eccezioni, le registra e ritorna. Non
rilancia.

Non vale per i publisher verso il bus: quelli devono poter fallire e far riconsegnare il messaggio.

## Why

Le proiezioni girano dentro la sottoscrizione allo stream globale, che è sequenziale. Un'eccezione che
risale non perde un solo documento: ferma la sottoscrizione, e da quel momento **ogni** evento
successivo smette di essere proiettato — per tutti i read model, non solo per quello rotto.

Il sintomo è una dashboard che si congela su uno stato vecchio senza che nulla dica perché.

## Enforcement

Revisione. Ogni `DomainEventHandlerAsync` che scrive su Mongo ha il proprio `try`/`catch` con un log
che nomina l'aggregato coinvolto.
