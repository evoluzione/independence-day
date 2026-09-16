# independence-day

Gli alieni attaccano, la Terra si difende, e in mezzo c'è una **saga** che tiene in piedi il processo.
La saga non c'è: c'è il posto dove scriverla. È quella la sfida.

Non è una sfida di strategia. Non c'è niente da ottimizzare, non ci sono forze da dosare: quelle
decisioni le prende la Terra, che è l'unica a conoscerle. Quello che devi far funzionare è il
processo distribuito — **gli ordini che si perdono, i pezzi che si rompono, e quello che hai preso in
prestito e devi restituire**.

Sotto ci sono tre servizi che non si conoscono — parlano solo via bus, ognuno con il proprio event
store e il proprio read model — su Muflone, CQRS ed event sourcing.

## Si parte

```bash
docker compose up --build
```

Poi **<http://localhost:8080>** e *Inizia la campagna*.

Non serve altro: né .NET, né Node, né una base dati. I tre servizi vengono compilati dentro l'immagine
SDK, quindi sulla macchina bastano Docker e questo repository.

| | |
| --- | --- |
| Sala operativa | <http://localhost:8080> |
| Log del tuo processo | `docker compose logs -f saga` |
| Test | `docker compose --profile test run --rm tests` |
| API dello spazio | <http://localhost:8090/space/ships> |
| RabbitMQ | <http://localhost:15673> — guest / guest |
| KurrentDB | <http://localhost:2114> (spazio) · <http://localhost:2115> (terra) |

Le porte pubblicate stanno fuori dagli standard di proposito: è normale avere già un Mongo o un Rabbit
in ascolto, e un conflitto di porta si presenta come un servizio che non parte.

## La sfida

Scrivi il processo che difende la Terra. Vince chi supera il **livello 10**.

```
src/Sagas/Evoluzione.IndependenceDay.Sagas/ShipInterception/
```

Così com'è, la saga si avvia e non gestisce nessun evento: nessun cannone spara, ogni nave tocca
terra, e le prime città cadono già alla prima ondata.

Cinque città, **un cannone ciascuna**, 100 colpi a testa che non si ricaricano mai. Una nave che
tocca terra rade al suolo la città, quindi vanno fermate tutte. Tu chiedi di aprire il fuoco su una
nave; quale cannone e quanti colpi lo decide la Terra. Poi devi fare le
quattro cose che nessuno farà al posto tuo:

| | |
| --- | --- |
| **Insistere** | un ordine su tredici si perde, e non produce nessun evento. Te ne accorgi solo dal battito |
| **Riparare** | un cannone si inceppa ogni nove colpi e non si sblocca da solo |
| **Restituire** | un cannone acceso continua a sparare su una nave già caduta, finché non lo spegni |
| **Chiudere** | il processo non finisce quando la nave cade, ma quando il conto con la Terra è chiuso |

L'ultima è quella su cui si perde la partita, ed è l'unica che non dà nessun errore quando la
sbagli.

- **[Le regole](REGOLE.md)** — cannoni, navi, guasti, i dieci livelli
- **[Comandi ed eventi](EVENTI.md)** — cosa puoi mandare, cosa ti arriva, come si aggancia
- **[L'architettura](ARCHITETTURA.md)** — i tre servizi e dove sta cosa

## I test sono la specifica

```bash
docker compose --profile test run --rm tests
```

`Sagas.Tests` è rosso, e ogni test rosso descrive un passo del processo. Falli diventare verdi in
ordine: quando lo sono tutti, la campagna si vince.

L'ultimo è una **simulazione della campagna intera** in memoria — niente bus, niente Mongo — e viene
in coppia con il suo contrario: *chi non restituisce i cannoni perde*. Se quel secondo test diventa
verde, la compensazione è diventata decorativa e i numeri del gioco sono sbagliati.

## Su Apple Silicon

KurrentDB pubblica solo immagini x64 sui tag stabili: i due container girano in emulazione e partono
più lenti. Per il nativo, in un `docker-compose.override.yml` (non versionato):

```yaml
services:
  eventstore-space: { image: kurrentplatform/kurrentdb:26.1.2-experimental-arm64-10.0-noble, platform: linux/arm64 }
  eventstore-earth: { image: kurrentplatform/kurrentdb:26.1.2-experimental-arm64-10.0-noble, platform: linux/arm64 }
```

## La soluzione

Il branch `soluzione` contiene il processo completo, che supera tutti e dieci i livelli. Guardalo
solo dopo aver provato il tuo: il diff con `main` è di pochi file, ed è esattamente l'esercizio.

```bash
git checkout soluzione
```
