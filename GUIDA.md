# Guida

Come si comincia, in sette passi.

## 1. Accendi tutto

```bash
docker compose up --build
```

Apri **[http://localhost:8080](http://localhost:8080)** e premi *Inizia la campagna*.

Guarda cosa succede: le navi arrivano, nessun cannone spara, tutte e cinque le città cadono. È il
punto di partenza — la saga esiste ma non fa niente.

## 2. Guarda i test

```bash
docker compose --profile test run --rm tests
```

Se hai .NET installato, o se stai lavorando dentro il dev container, usa invece `dotnet test src/Evoluzione.IndependenceDay.slnx`: è la stessa cosa, ma parte subito. Le
[tre strade](README.md#compilare-con-o-senza-net-installato) sono nel README.

Cinque test rossi, uno per problema. **Sono la specifica**: il nome dice qual è il problema, il
commento dice perché, e l'asserzione dice esattamente cosa deve finire sul bus.

Non è un cancello: ogni test in più fa abbattere **più navi** dell'ultimo, e si vede nel resoconto di
fine ondata. Falli diventare verdi in ordine — dal primo, che è la base di tutti gli altri.

## 3. Apri i due file

```
src/Sagas/Evoluzione.IndependenceDay.Sagas/ShipInterception/
├── InterceptionState.cs      ← quello che il processo si ricorda fra un evento e l'altro
└── ShipInterceptionSaga.cs   ← quello che il processo fa
```

Sono gli unici due file da scrivere. Gli handler ci sono già tutti e non fanno niente.

Lo stato arriva vuoto, e **la domanda da farsi prima di metterci un campo è sempre la stessa: questa
cosa la sa già la Terra?** Quasi sempre sì — quali cannoni stiano sparando a una nave, se la nave sia
ancora in volo, quanti colpi restino. Ricordarsela qui vuol dire tenerne una seconda copia, che
prima o poi dirà una cosa diversa dall'originale. La soluzione di riferimento lascia
`InterceptionState` vuoto dal primo test all'ultimo: se ti accorgi di volerci scrivere qualcosa,
rileggi l'ordine che stavi per mandare — probabilmente c'è un modo di chiederlo che non richiede di
ricordare.

## 4. Fai verde il primo test

`Una_nave_avvistata_apre_il_fuoco_e_si_insiste_se_resta_scoperta` vuole due cose: quando il processo
parte, manda `OpenFire`; e quando il battito dice che la nave è ancora scoperta, insiste.

In `StartedByAsync`, dopo aver salvato lo stato, spedisci l'ordine con `SendCommand`. `OpenFire` è
già lì in fondo al file, pronto, e vuole solo l'id della nave: sta nel comando che ti ha fatto
partire.

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
a mano — e il gioco resta fermo com'era.

Se un comportamento funziona nei test e non nel gioco, guarda qui per primo.

## 6. Rigioca

Il container della saga sta gia' guardando i tuoi file: al salvataggio ricompila e riparte da solo,
non c'e' niente da rilanciare. Il riavvio si vede qui:

```bash
docker compose logs -f saga
```

Poi *Inizia la campagna*, e guarda quante navi abbatti. Dovrebbero crescere con ogni test verde.

Quando si rompe, hai tre posti dove guardare:

|                                 |                                                                  |
| ------------------------------- | ---------------------------------------------------------------- |
| La console                      | il diario a destra racconta ogni passo di ogni nave, con l'esito |
| `docker compose logs -f saga` | quello che il tuo processo decide, evento per evento             |
| Il resoconto di fine ondata     | città, navi atterrate, colpi rimasti, cannoni lasciati accesi   |

## 7. Ripeti fino a cinque

Ogni test aggiunge un comportamento, e ogni comportamento abbatte più navi:

|                                   |                                                                                        |
| --------------------------------- | -------------------------------------------------------------------------------------- |
| **1. Aprire**               | chiedi un cannone all'avvistamento, e insisti se il battito dice che sei scoperto      |
| **2. Restituire**           | i cannoni sono un prestito: restituiscili quando la nave cade                          |
| **3. Riparare**             | un cannone inceppato non si sblocca da solo                                            |
| **4. Rimettere in azione**  | riparare non riapre il fuoco: è una mossa a parte                                      |
| **5. Rifornire**            | un cannone a secco chiama il convoglio, e alla consegna torna in azione                |

Quando sono verdi tutti e cinque, la campagna si vince.

---

## Le quattro cose da sapere prima di cominciare

**Un cannone può restare scoperto.** I cannoni sono cinque e le navi arrivano di più: una nave può
non trovarne uno libero all'avvistamento. L'unico modo di accorgersene è il battito.

**Aprire il fuoco è un prestito.** Il cannone resta acceso finché non lo restituisci, anche dopo che
la nave è caduta.

**Riparare e rifornire non riaprono il fuoco.** Rimettono il cannone disponibile, e fermo.
Rimetterlo in azione è sempre una mossa a parte.

**Qui non va una riga di dominio.** Quanti colpi regga una corazzata, quale cannone convenga, quanto
duri una ricarica: non lo sai e non devi saperlo. Tu chiedi, la Terra decide.

## Dove guardare

- **[Le regole](REGOLE.md)** — i numeri, i guasti, l'ondata
- **[Comandi ed eventi](EVENTI.md)** — cosa puoi mandare e cosa ti arriva
- **[L&#39;architettura](ARCHITETTURA.md)** — i tre servizi e dove sta cosa

## Se ti blocchi

C'è un branch con la soluzione, `soluzione`. Guardalo solo dopo aver provato la tua — e nota che
`InterceptionState` non compare nel diff: nella soluzione resta vuoto.

```bash
git diff main soluzione -- src/Sagas/
```
