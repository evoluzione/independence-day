# Comandi ed eventi

Tutto quello che il tuo processo può mandare, e tutto quello che gli arriva. Stanno in
`src/Shared/Evoluzione.IndependenceDay.Contracts/`: sono il contratto fra i servizi, e l'unico
vocabolario che hai.

## Quello che mandi — tre ordini

| Comando | Cosa fa | Cosa torna indietro |
| --- | --- | --- |
| `OpenFire(shipId)` | accende un cannone su quella nave, e lo lascia acceso | `FireOpened(cityId)` oppure `NoCannonReady` |
| `CeaseFire(cityId, shipId)` | lo spegne e lo restituisce | `FireCeased` |
| `RepairCannon(cityId, shipId)` | rimette in sesto un cannone inceppato | `CannonRepaired` |

Tre ordini, e nessuno dice *quale* cannone o *quanti* colpi: quelle sono decisioni della Terra.

Ogni ordine va all'aggregato `EarthDefense`, con `Cities.DefenseId` come aggregato e il
`CorrelationId` del tuo processo — è quello il filo che riporta l'esito a te e non a un altro.

**Un ordine che arriva ed è ancora sensato produce sempre uno di questi eventi.** Non esiste un
quarto esito che sia "niente".

Esistono però due modi di non ricevere nessuna risposta, e sono cose diverse:

| | |
| --- | --- |
| **L'ordine non è arrivato** | uno su venticinque si perde sul collegamento. Sulla Terra non succede niente, quindi non c'è niente da raccontare: nessun evento, nemmeno un rifiuto. **Non è un esito dell'ordine, è la sua assenza** — e l'unico modo di accorgersene è il battito |
| **L'ordine non aveva più senso** | un cessate il fuoco su un cannone che nel frattempo è stato messo su un'altra nave, o la riconsegna di un ordine già eseguito. L'aggregato esce in silenzio perché non c'è niente da fare, ed è la stessa cosa che avrebbe fatto la seconda volta |

Quello che un aggregato **non** fa mai è scartare un ordine che ha ancora senso. Se lo facesse
sarebbe una rete che finge, e non sapresti più distinguere i due casi qui sopra.

## Quello che ti arriva

### L'avvio

| Evento | Quando |
| --- | --- |
| `ShipDetected(shipId, cityId)` | la Terra ha preso in carico una nave. **Da qui parte il processo**, e parte il cronometro degli 8 secondi |

Non si parte dall'avvistamento dello Spazio (`AlienShipDetected`): prima della presa in carico la
Terra non accetta ordini su quella nave.

### Gli esiti degli ordini

| Evento | Cosa dice |
| --- | --- |
| `FireOpened(cityId, shipId, roundsLeft)` | quel cannone è tuo finché non lo restituisci |
| `NoCannonReady(shipId)` | tutti impegnati, rotti o a secco. Temporaneo: riprova |
| `FireCeased(cityId, shipId, roundsLeft)` | il cannone è tornato. **È la conferma della compensazione** |
| `CannonRepaired(cityId, shipId, roundsLeft)` | disponibile, non in azione: riparare non riapre il fuoco |

### I guasti

| Evento | Cosa dice |
| --- | --- |
| `CannonJammed(cityId, shipId)` | il cannone si è fermato. Non si ripara da solo |
| `CannonEmpty(cityId, shipId)` | ha finito i colpi. Per sempre |

### Gli esiti della nave

| Evento | Cosa dice |
| --- | --- |
| `ShipDestroyed(shipId, cityId)` | abbattuta. **I cannoni continuano a sparare** |
| `ShipLanded(shipId, cityId, damage, integrityLeft)` | ha toccato terra |
| `CityFallen(cityId, shipId)` | integrità a zero: città e cannone perduti |

### Il battito — ogni mezzo secondo

| Evento | Cosa dice |
| --- | --- |
| `ShipApproaching(shipId, cityId, msToImpact, cannonsFiring)` | la nave è ancora viva, e quanti le stanno sparando |
| `CannonStillFiring(cityId, shipId)` | questo cannone spara a una nave che non c'è più |

Sono gli unici due eventi che **arrivano senza che tu abbia chiesto niente**, e gli unici che possano
rivelare un ordine perso. Un processo che non li ascolta funziona finché non si perde il primo
ordine, e poi smette di funzionare in silenzio.

## Il giro completo, quando tutto va bene

```
AlienShipDetected      (Spazio → Terra)
  ShipDetected         (Terra → tu)          ──►  OpenFire
  FireOpened                                      … il cannone spara da solo, ogni 400 ms
  ShipApproaching × n  (battito)
  ShipDestroyed                              ──►  CeaseFire
  FireCeased                                      processo chiuso
```

E quando qualcosa va storto:

```
OpenFire            ──►  ✗ perso sul collegamento, non arriva mai
  ShipApproaching   cannonsFiring: 0         ──►  OpenFire          ← si insiste
  FireOpened
  CannonJammed                               ──►  RepairCannon      ← si ripara
  CannonRepaired
  ShipApproaching   cannonsFiring: 0         ──►  OpenFire
  ShipDestroyed                              ──►  CeaseFire
                    ──►  ✗ perso anche questo
  CannonStillFiring                          ──►  CeaseFire         ← si richiude
  FireCeased                                      processo chiuso
```

## Come si aggancia un evento

Quattro passi, e saltarne uno **non dà errore**: l'evento semplicemente non arriva mai, e il processo
resta fermo sul gradino precedente.

1. **La saga lo dichiara** — `ISagaEventHandlerAsync<TEvento>` su `ShipInterceptionSaga`, con un
   `HandleAsync` da una riga che chiama `Advance`.
2. **Arriva dal bus** — `services.AddIntegrationEventHandler<SagaIntegrationEventHandler<TEvento>>()`
   in `Sagas.Facade/ExtensionsHelper.cs`.
3. **Arriva al processo** — `services.AddSagaEventHandler<TEvento, ShipInterceptionSaga>()`, stesso file.
4. **Il processo lo gestisce** — un ramo nello `switch` di `InterceptionProcess.React`.

I primi tre sono trasporto, il quarto è la decisione. Sono separati apposta: quello che si legge
nella saga è **quali eventi attraversano il processo**, il perché sta altrove.

## Cosa non ti arriva, e non è una dimenticanza

| | |
| --- | --- |
| Un evento per l'ordine perso | non esiste. L'ordine non è mai arrivato: non c'è niente da raccontare |
| `EarthShotFired` · `EarthShotMissed` · `EarthShotWasted` | cronaca del tiro colpo per colpo: riguarda la pagina, non te. Un bersaglio mancato non ti riguarda perché non c'è niente da fare — il cannone riprova da solo |
| Quanti colpi serva una stazza | è un conto di dominio, e il dominio è la Terra |
| Quanti colpi restano a un cannone | te lo dicono `FireOpened` e `FireCeased`, per quel cannone |
| Cosa stanno facendo gli altri processi | niente. Un processo vive per una nave sola |
