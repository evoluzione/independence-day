# R-9030: Un evento di dominio porta il nome del proprio aggregato

- Status: enforced
- Source: [ADR-9000](../adr/9000-service-boundaries.md)

## Rule

Gli eventi che un contesto scrive nel proprio event store si chiamano come l'aggregato che li ha
emessi: `CityShieldsRaised`, `CityInterceptorsLost`, `InvasionWaveStarted`.

I nomi **puliti** restano ai contratti in `Contracts.Events`, che sono la lingua condivisa fra i
servizi: `ShieldsRaised`, `InterceptorsLost`, `InvasionStarted`.

Nessun tipo di `Contracts.Events` puo' avere lo stesso nome semplice di un evento di dominio di un
qualunque contesto.

## Why

Muflone instrada i messaggi per **nome semplice** del tipo, non per nome completo. Due omonimi in
spazi dei nomi diversi finiscono sulla stessa coda, e il consumatore riceve un messaggio che non sa
deserializzare.

Il difetto non e' rumoroso dove fa male: il servizio registra un errore di deserializzazione fra gli
altri e intanto il consumatore che aspettava quell'evento resta indietro. Si presenta come un flusso
che si ferma, non come un conflitto di nomi.

La direzione della disambiguazione non e' casuale. Il nome pulito va al contratto perche' e' quello
che leggono tre servizi; il prefisso va all'evento di dominio, dove non e' rumore ma informazione —
dice a quale aggregato appartiene, che e' la prima cosa da sapere leggendo uno stream.

## Enforcement

Revisione. Il controllo e' locale: quando si aggiunge un evento a `Contracts.Events`, si guarda che
il nome non esista gia' in `Earth.Messages.Events` o `Space.Messages.Events` — e visto che quelli
portano tutti il nome del proprio aggregato, non dovrebbe succedere mai.
