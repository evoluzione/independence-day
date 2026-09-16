# R-9010: Una regola di dominio si rifiuta con un evento, non con un'eccezione

- Status: enforced
- Source: [ADR-9020](../adr/9020-saga-owns-the-coordination.md), [ADR-9060](../adr/9060-deterministic-combat.md)

## Rule

Un aggregato non solleva un'eccezione quando una regola di business non è soddisfatta. Ha due risposte
possibili:

- **Esce in silenzio** quando il comando non ha più senso: una nave già abbattuta a cui arriva un
  secondo ordine di distruzione è una riconsegna, non un errore.
- **Emette l'evento negativo** quando qualcuno sta aspettando una risposta: nessun cannone libero,
  cannone inceppato, munizioni finite — `EarthNoCannonReady`, `EarthCannonJammed`, `EarthCannonEmpty`.

Restano legittime le eccezioni sui dati malformati — un identificativo vuoto, zero navi — perché lì non
c'è nessuna decisione di dominio da prendere.

## Why

La saga è un punto di passaggio obbligato: manda un comando e aspetta il suo esito. Un aggregato che
solleva fa fallire il consumo del messaggio e la saga non riceve niente; un aggregato che esce in
silenzio dove qualcuno sta aspettando la lascia ferma sul gradino precedente per sempre.

Entrambi i casi hanno la stessa forma dal di fuori — la difesa non prosegue — e nessuno dei due si
presenta come un errore. Per questo la distinzione fra "nessuno aspetta" e "qualcuno aspetta" è la
regola, e non una questione di stile.

Il caso di riferimento è una città senza intercettori: emette `InterceptorsLost` con perdita zero
invece di non emettere niente, ed è quello che permette alla saga di salire al gradino del virus.

## Enforcement

Revisione, più un test per ogni ramo di rifiuto. Il test `DeployNukes_WhenTheReserveIsEmpty_RefusesTheOrder` esiste per questo.
