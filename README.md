# independence-day

Gli alieni hanno deciso di invadere la terra (stranamente per primi gli Stati Uniti) e tu sei nel team di Difesa Intergalattica, è arrivato l'Independence Day! Dalla sala di controllo dovrai reagire agli eventi e lanciare dei comandi, il kata sta nel costruire una Saga che possa salvare il pianeta dall'incombente minaccia.

Gli ordini si possono perdere per strada, i proiettili possono andare sprecati o mancare il bersaglio, i cannoni possono addirittura incepparsi. Il destino della terra è nelle tue mani.

Sotto ci sono tre servizi che non si conoscono — parlano solo via bus, ognuno con il proprio event
store e il proprio read model — su Muflone, CQRS ed event sourcing.

## Si parte

```bash
docker compose up --build
```

Poi **[http://localhost:8080](http://localhost:8080)** e *Inizia la campagna*.

Non serve altro: né .NET, né Node, né una base dati. I servizi vengono compilati dentro l'immagine
SDK, quindi sulla macchina bastano **Docker** e **Git**.

I due file da scrivere, che sono tutto l'esercizio:

```
src/Sagas/Evoluzione.IndependenceDay.Sagas/ShipInterception/
```

Il container della saga gira sotto `dotnet watch` con `src/` montato dentro: **salvi il file e
ricompila e riparte da solo**, senza rilanciare `docker compose`. Errori di compilazione compresi —
il container resta in piedi e aspetta, correggi e risalva.

```bash
docker compose logs -f saga
```

Tienilo aperto in un terminale a parte: è lì che si vede il riavvio, e da lì in poi è la finestra sul
tuo processo mentre gioca.

👉 **[La guida](GUIDA.md)** spiega i passi uno per uno. Comincia da lì.

## Compilare, con o senza .NET installato

Far girare il gioco non richiede .NET. Per **scrivere** i due file ci sono tre strade: scegli la tua,
il kata è identico in tutte e tre.

### Hai l'SDK .NET 10 sulla macchina

Apri la cartella in VS Code e basta, IntelliSense c'è già. I test girano nativi, ed è la via più
rapida — nessun container di mezzo:

```bash
dotnet test src/Evoluzione.IndependenceDay.slnx
```

### Non hai .NET e non vuoi installarlo — dev container

Serve l'estensione [Dev
Containers](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers).
Apri la cartella, poi *Reopen in Container* dal toast che compare (o F1 → `Dev Containers: Reopen in
Container`).

VS Code riaggancia la finestra a un container che ha l'SDK dentro: l'editor resta sul tuo PC, il
compilatore no. La prima volta scarica ~1 GB e fa il restore, una volta sola. Da lì il terminale
integrato **è dentro il container**, quindi:

```bash
dotnet test src/Evoluzione.IndependenceDay.slnx
```

Il gioco invece si accende **da un terminale del tuo PC**, non da quello di VS Code: dentro il dev
container non c'è Docker.

### Non vuoi né .NET né il dev container

I test girano lo stesso, nell'immagine SDK, senza installare niente:

```bash
docker compose --profile test run --rm tests
```

Nessun IntelliSense, ma i test sono la specifica e bastano a chiudere il kata.

## I test

Comunque tu li lanci, sono rossi e raggruppati in **tre gradini**, uno per livello. Falli diventare
verdi in ordine: con i gradini fino a N verdi la campagna supera i primi N livelli, e senza il
gradino giusto al livello dopo si perdono tutte e cinque le città.

## Dove sta cosa

|                      |                                                                                                                 |
| -------------------- | --------------------------------------------------------------------------------------------------------------- |
| Sala operativa       | [http://localhost:8080](http://localhost:8080)                                                                   |
| Log del tuo processo | `docker compose logs -f saga`                                                                                 |
| Test                 | `dotnet test src/Evoluzione.IndependenceDay.slnx`, oppure `docker compose --profile test run --rm tests`          |
| API dello spazio     | [http://localhost:8090/space/ships](http://localhost:8090/space/ships)                                           |
| RabbitMQ             | [http://localhost:15673](http://localhost:15673) — guest / guest                                                |
| KurrentDB            | [http://localhost:2114](http://localhost:2114) (spazio) · [http://localhost:2115](http://localhost:2115) (terra) |

Le porte pubblicate stanno fuori dagli standard di proposito: è normale avere già un Mongo o un Rabbit
in ascolto, e un conflitto di porta si presenta come un servizio che non parte.

- **[La guida](GUIDA.md)** — come si comincia, in sette passi
- **[Le regole](REGOLE.md)** — cannoni, navi, guasti, i tre livelli
- **[Comandi ed eventi](EVENTI.md)** — cosa puoi mandare, cosa ti arriva, come si aggancia
- **[L&#39;architettura](ARCHITETTURA.md)** — i tre servizi e dove sta cosa

## Su Apple Silicon

KurrentDB pubblica solo immagini x64 sui tag stabili: i due container girano in emulazione e partono
più lenti. Per il nativo, in un `docker-compose.override.yml` (non versionato):

```yaml
services:
  eventstore-space: { image: kurrentplatform/kurrentdb:26.1.2-experimental-arm64-10.0-noble, platform: linux/arm64 }
  eventstore-earth: { image: kurrentplatform/kurrentdb:26.1.2-experimental-arm64-10.0-noble, platform: linux/arm64 }
```

## La soluzione

C'è un branch per ogni gradino, `livello-01`, `livello-02`, `livello-03`, con la saga scritta **fino
a quel livello**. Guardali solo dopo aver provato la tua: il diff fra due consecutivi è esattamente
quello che aggiunge quel gradino, e niente di più.

```bash
git diff livello-01 livello-02 -- src/Sagas/
```

`livello-03` è la saga completa, che supera tutti e tre i livelli con cinque città in piedi e nessuna
nave a terra. Il diff con `main` è di **due file**, ed è esattamente l'esercizio.

```bash
git diff main livello-03 -- src/Sagas/
```
