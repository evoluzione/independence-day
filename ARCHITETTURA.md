# L'architettura

Tre servizi che non si conoscono. Nessuno chiama nessuno: si parlano solo con messaggi su RabbitMQ,
e ognuno ha il proprio event store e il proprio read model.

```
                    ┌──────────────────────────────── RabbitMQ ────────────────────────────────┐
                    │                                                                          │
   ┌────────────────┴──────────┐   ┌──────────────────────────┐   ┌───────────────────────────┴──┐
   │  SPAZIO                   │   │  SAGHE                   │   │  TERRA                       │
   │  manda le navi            │   │  tiene in piedi il       │   │  cannoni, colpi, combatti-   │
   │                           │   │  processo                │   │  mento, e la sala operativa  │
   │  AlienShip · Invasion     │   │  ShipInterceptionSaga    │   │  EarthDefense                │
   │  KurrentDB :2114          │   │  Mongo (stato saghe)     │   │  KurrentDB :2115             │
   │  Mongo (read model)       │   │  nessun event store      │   │  Mongo (read model) · :8080  │
   └───────────────────────────┘   └──────────────────────────┘   └──────────────────────────────┘
```

| Servizio | Cosa possiede | Cosa non sa |
| --- | --- | --- |
| **Spazio** | quante navi, di che stazza, quando partono | che esista una difesa |
| **Terra** | cannoni, munizioni, combattimento, guasti | cosa succederà degli esiti che racconta |
| **Saghe** | il filo di ogni intercettazione | tutto il resto: nessuna regola di gioco vive qui |

## Il giro di un'intercettazione

```
Spazio      AlienShipLaunched  ──►  AlienShipDetected ─────────────┐
                                                                   ▼
Terra                                          DetectShip ──► EarthShipDetected
                                                                   │
Saghe                                         ShipDetected ◄───────┘
                                                    │
                                              StartShipInterception
                                                    │
                                                OpenFire ──────────┐
                                                                   ▼
Terra                                                        EarthFireOpened
                                                       ╔═══════════════════════╗
                                                       ║  FireControl: un      ║
                                                       ║  colpo ogni 400 ms,   ║
                                                       ║  finché non si spegne ║
                                                       ╚═══════════════════════╝
                                                                   │
Saghe                                           ShipDestroyed ◄────┘
                                                    │
                                                CeaseFire ─────────┐
                                                                   ▼
Terra                                                        EarthFireCeased
Saghe                                              FireCeased ◄────┘   chiuso
```

## Dove sta cosa

```
src/
├── Shared/
│   ├── Contracts/          il vocabolario comune: comandi, eventi, id, e le regole del mondo
│   │   └── World/          Armory (colpi, ricarica, guasti) · Ships (stazze) · Cities · Invasion
│   └── Infrastructure/     Mongo, KurrentDB, RabbitMQ: montaggio, non dominio
├── Space/
│   ├── .Domain/            AlienShip · Invasion · WaveDifficulty (la curva dei dieci livelli)
│   ├── .Messages/          i suoi comandi ed eventi, privati
│   ├── .ReadModel/         proiezioni, e i publisher che portano i fatti sul bus
│   └── .Facade/            InvasionGenerator: manda una nave ogni 700 ms
├── Earth/
│   ├── .Domain/            EarthDefense: cinque cannoni, le navi in volo, i guasti
│   ├── .Messages/          i suoi comandi ed eventi, privati
│   ├── .ReadModel/         proiezioni, publisher, e lo snapshot che la pagina legge
│   ├── .Facade/            FireControl · ApproachDeadline · Heartbeat · gli endpoint
│   └── .Host/wwwroot/      la sala operativa: una pagina, alimentata da server-sent events
└── Sagas/
    ├── .Sagas/             ◄── QUI. ShipInterceptionSaga, InterceptionProcess, InterceptionState
    ├── .Facade/            le registrazioni: due righe per ogni evento che vuoi ascoltare
    └── .Infrastructure/    lo stato delle saghe su Mongo
```

## Le tre regole che spiegano il resto

**Un aggregato non solleva mai.** Un ordine impossibile non è un errore del chiamante, è un fatto
della battaglia: la Terra risponde con un evento. Vedi
[R-9010](docs/rules/9010-aggregates-do-not-throw.md).

**Chi possiede la risorsa decide.** Quale cannone spari, quanti colpi servano, quando si inceppa: lo
sa la Terra, e resta sulla Terra. Il progetto saghe non contiene nessuna regola di gioco. Vedi
[ADR-9070](docs/adr/9070-the-owner-decides.md).

**Il tempo lo conosce chi esegue.** La ricarica sta in `FireControl`, la scadenza in
`ApproachDeadline`, il battito in `Heartbeat`. Sono tre giri di fondo sulla Terra, e chi coordina non
ha nessun timer.

## I giri di fondo

| Servizio | Ogni | Cosa fa |
| --- | ---: | --- |
| `InvasionGenerator` | 500 ms | manda la prossima nave del piano (una al secondo), chiude l'ondata quando è vuota |
| `FireControl` | 100 ms | preme il grilletto ai cannoni che hanno finito di ricaricare |
| `ApproachDeadline` | 500 ms | dopo 8 secondi fa toccare terra a quello che resta |
| `Heartbeat` | 500 ms | racconta navi vive e cannoni rimasti accesi su relitti |

Il battito è l'unico che non tocca nessun aggregato: non è un fatto di dominio, è un resoconto letto
dal read model e messo sul bus com'è.

## Per approfondire

- [docs/adr/](docs/adr/) — le decisioni, e perché non sono state prese le alternative
- [docs/rules/](docs/rules/) — quello che in revisione è un difetto
- [docs/guidelines/](docs/guidelines/) — come si scrive qui dentro
