# Guida

Come si comincia, in sette passi.

## 1. Accendi tutto

```bash
docker compose up --build
```

Apri **[http://localhost:8080](http://localhost:8080)** e premi *Inizia la campagna*.

Guarda cosa succede: cinque navi arrivano, nessun cannone spara, cinque città cadono. È il punto di
partenza — la saga esiste ma non fa niente.

## 2. Guarda i test

```bash
docker compose --profile test run --rm tests
```

Se hai .NET installato, o se stai lavorando dentro il dev container, usa invece `dotnet test src/Evoluzione.IndependenceDay.slnx`: è la stessa cosa, ma parte subito. Le
[tre strade](README.md#compilare-con-o-senza-net-installato) sono nel README.

Test rossi, raggruppati in **tre gradini**, uno per livello. **Sono la specifica**: il nome dice a
che livello servono, il commento dice perché, e l'asserzione dice esattamente cosa deve finire sul
bus.

Vanno fatti diventare verdi **in ordine**. Con i gradini fino a N verdi la campagna supera i primi N
livelli senza perdere una città, e si ferma al successivo. Non per un pelo: senza il gradino giusto
al livello dopo non perdi una città, **le perdi tutte e cinque**.

## 3. Apri i due file

```
src/Sagas/Evoluzione.IndependenceDay.Sagas/ShipInterception/
├── InterceptionState.cs      ← quello che il processo si ricorda fra un evento e l'altro
└── ShipInterceptionSaga.cs   ← quello che il processo fa
```

Sono gli unici due file da scrivere. Gli handler ci sono già tutti e non fanno niente.

## 4. Fai verde il primo test

`Livello_1_la_prima_mossa_e_aprire_il_fuoco` vuole una cosa sola: quando il processo parte, manda
`OpenFire`.

In `StartedByAsync`, dopo aver salvato lo stato, spedisci l'ordine con `SendCommand`. `OpenFire` è
già lì in fondo al file, pronto.

```bash
docker compose --profile test run --rm tests
```

Primo verde.

## 5. Aggancia gli eventi — è il passo che si dimentica

Dal secondo test in poi il processo deve **ricevere** qualcosa, e ricevere non è automatico. Ogni
evento vuole **due righe** in `src/Sagas/Evoluzione.IndependenceDay.Sagas.Facade/ExtensionsHelper.cs`:

```csharp
services.AddIntegrationEventHandler<SagaIntegrationEventHandler<ShipDestroyed>>();
services.AddSagaEventHandler<ShipDestroyed, ShipInterceptionSaga>();
```

La prima lo fa arrivare dal bus, la seconda lo consegna al processo. **Dimenticarle non dà nessun
errore**: l'evento semplicemente non arriva mai, i test unitari restano verdi — loro chiamano la saga
a mano — e il gioco resta fermo sul gradino prima.

Se un comportamento funziona nei test e non nel gioco, guarda qui per primo.

## 6. Rigioca

```bash
docker compose up --build
```

*Inizia la campagna*, e guarda fin dove arrivi. Dovresti superare tanti livelli quanti test hai
verdi.

Quando si rompe, hai tre posti dove guardare:

|                                 |                                                                  |
| ------------------------------- | ---------------------------------------------------------------- |
| La console                      | il diario a destra racconta ogni passo di ogni nave, con l'esito |
| `docker compose logs -f saga` | quello che il tuo processo decide, evento per evento             |
| Il resoconto di fine ondata     | città, navi atterrate, colpi rimasti, cannoni lasciati accesi   |

## 7. Ripeti fino a tre

Ogni gradino aggiunge un comportamento, e ogni comportamento sblocca un livello:

|                                   |         |                                                                                        |
| --------------------------------- | ------- | -------------------------------------------------------------------------------------- |
| **1. Aprire**               | 5 navi  | chiedi un cannone, e qualcosa comincia a sparare                                       |
| **2. Restituire e reagire** | 10 navi | il cannone è un prestito, gli ordini si perdono in silenzio, e i cannoni si inceppano |
| **3. Chiudere**             | 15 navi | il processo finisce quando il conto è saldato, non quando la nave cade                |

Quando sono verdi tutti e tre, la campagna si vince.

---

## Le quattro cose da sapere prima di cominciare

**Un ordine può non arrivare.** Uno su venticinque si perde, e un ordine perso **non produce nessun
evento** — nemmeno un errore. Chi aspetta un fallimento aspetta per sempre. L'unico modo di
accorgersene è il battito.

**Aprire il fuoco è un prestito.** Il cannone resta acceso finché non lo restituisci, anche dopo che
la nave è caduta. E il cessate il fuoco può perdersi come tutto il resto, quindi va **verificato**.

**Il processo non finisce quando la nave cade.** Finisce quando il conto con la Terra è chiuso. È qui
che si perde la partita, ed è l'unico errore che non dà nessun segnale.

**Qui non va una riga di dominio.** Quanti colpi regga una corazzata, quale cannone convenga, quanto
duri una ricarica: non lo sai e non devi saperlo. Tu chiedi, la Terra decide.

## Dove guardare

- **[Le regole](REGOLE.md)** — i numeri, i guasti, i tre livelli
- **[Comandi ed eventi](EVENTI.md)** — cosa puoi mandare e cosa ti arriva
- **[L&#39;architettura](ARCHITETTURA.md)** — i tre servizi e dove sta cosa

## Se ti blocchi

C'è un branch per ogni gradino — `livello-01`, `livello-02`, `livello-03` — con la soluzione **fino
a quel livello**. Se sei fermo al secondo, guarda solo il secondo:

```bash
git diff livello-01 livello-02 -- src/Sagas/
```

Il diff fra due branch consecutivi è esattamente quello che aggiunge quel gradino, e niente di più.
Per vedere tutto quello che serve fino a un certo punto, confronta con `main`:

```bash
git diff main livello-02 -- src/Sagas/
```

`livello-03` è la saga completa. Guardala solo dopo aver provato la tua: il diff con `main` è di due
file, ed è esattamente l'esercizio.
