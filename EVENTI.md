# Comandi ed eventi

Tutto quello che il tuo processo può mandare, e tutto quello che gli arriva. Stanno in
`src/Shared/Evoluzione.IndependenceDay.Contracts/`: sono il contratto fra i servizi, e l'unico
vocabolario che hai.

## Quello che mandi — quattro ordini

| Comando | Cosa fa | Cosa torna indietro |
| --- | --- | --- |
| `OpenFire(shipId)` | accende un cannone su quella nave, e lo lascia acceso | `FireOpened(cityId)` oppure `NoCannonReady` |
| `CeaseFire(shipId)` | spegne **ogni** cannone su quella nave e li restituisce | un `FireCeased(cityId)` per cannone |
| `RepairCannon(cityId, shipId)` | prova a rimettere in sesto un cannone inceppato | `CannonRepaired` |
| `RequestResupply(cityId, shipId)` | chiama il convoglio per un cannone a secco | `CannonResupplied` |

Quattro ordini, e nessuno dice *quale* cannone o *quanti* colpi: quelle sono decisioni della Terra.
`OpenFire` e `CeaseFire` non nominano nemmeno la città — parlano della **nave**, e la Terra traduce:
uno sceglie il cannone da accendere, l'altro spegne tutti quelli che erano su quella nave. Non è un
dettaglio di comodo: vuol dire che non devi tenere il conto di chi sta sparando a chi.

Ogni ordine va all'aggregato `EarthDefense`, con `Cities.DefenseId` come aggregato e il
`CorrelationId` del tuo processo — è quello il filo che riporta l'esito a te e non a un altro.

**Un ordine che arriva ed è ancora sensato produce sempre uno di questi eventi.** Non esiste un
quinto esito che sia "niente".

Esiste però un modo di non ricevere nessuna risposta: **l'ordine non aveva più senso** — un cessate
il fuoco su una nave su cui non spara più nessuno, un'apertura del fuoco su una nave già caduta o già
coperta, o la riconsegna di un ordine già eseguito. L'aggregato esce in silenzio perché non c'è
niente da fare, ed è la stessa cosa che avrebbe fatto la seconda volta.

Vale la pena fermarsi su questo: **è la Terra a sapere quando un ordine non ha più senso**, e lo sa
meglio di te. Un ordine mandato a vuoto non costa niente e non va evitato ricordandosi le cose.

Quello che un aggregato **non** fa mai è scartare un ordine che ha ancora senso. Se lo facesse
sarebbe una rete che finge.

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
| `CannonResupplied(cityId, shipId, rounds)` | carico, non in azione: come riparare, rifornire non riapre il fuoco |

### I guasti

| Evento | Cosa dice |
| --- | --- |
| `CannonJammed(cityId, shipId)` | il cannone si è fermato. Non si ripara da solo |
| `CannonEmpty(cityId, shipId)` | ha finito i colpi. **Si libera**: non è più assegnato a nessuna nave finché non torna carico |

### Gli esiti della nave

| Evento | Cosa dice |
| --- | --- |
| `ShipDestroyed(shipId, cityId)` | abbattuta. **I cannoni continuano a sparare** |
| `ShipLanded(shipId, cityId, damage, integrityLeft)` | ha toccato terra |
| `CityFallen(cityId, shipId)` | integrità a zero: città e cannone perduti |

### Il battito — ogni mezzo secondo

| Evento | Cosa dice |
| --- | --- |
| `ShipApproaching(shipId, cityId, msToImpact, cannonsFiring)` | la nave è ancora viva, e quanti cannoni le sono addosso |

È l'unico evento che **arriva senza che tu abbia chiesto niente**, e l'unico modo di scoprire che una
nave è rimasta scoperta — perché tutti i cannoni erano impegnati quando è arrivata, non perché un
ordine si sia perso da qualche parte.

`cannonsFiring` conta i cannoni **assegnati e in azione**: un cannone a secco non conta più, perché
si è già liberato da solo.

Il battito però è una rete, non il meccanismo. Quando un evento dice già tutto quello che serve — un
cannone riparato, un cannone rifornito — aspettare il battito costa mezzo secondo su otto di
finestra.

## Il giro completo, quando tutto va bene

```
AlienShipDetected      (Spazio → Terra)
  ShipDetected         (Terra → tu)          ──►  OpenFire
  FireOpened                                      … il cannone spara da solo, ogni 350 ms
  ShipApproaching × n  (battito)
  ShipDestroyed                              ──►  CeaseFire
  FireCeased                                      processo chiuso
```

E quando qualcosa va storto:

```
OpenFire
  ShipApproaching   cannonsFiring: 0         ──►  OpenFire          ← si insiste, il cannone era altrove
  FireOpened
  CannonJammed                               ──►  RepairCannon      ← si ripara
  CannonRepaired                             ──►  OpenFire          ← si rimette in azione, subito
  CannonEmpty                                ──►  RequestResupply   ← si chiama il convoglio
  CannonResupplied                           ──►  OpenFire          ← si rimette in azione, subito
  ShipDestroyed                              ──►  CeaseFire
  FireCeased                                      processo chiuso
```

## Come si aggancia un evento

Quattro passi, e saltarne uno **non dà errore**: l'evento semplicemente non arriva mai, e il processo
resta fermo com'era.

1. **La saga lo dichiara** — `ISagaEventHandlerAsync<TEvento>` nell'elenco su `ShipInterceptionSaga`.
2. **La saga lo gestisce** — un `HandleAsync` che chiama `Advance` con quello che c'è da fare.
3. **Arriva dal bus** — `services.AddIntegrationEventHandler<SagaIntegrationEventHandler<TEvento>>()`
   in `Sagas.Facade/ExtensionsHelper.cs`.
4. **Arriva alla saga** — `services.AddSagaEventHandler<TEvento, ShipInterceptionSaga>()`, stesso file.

Gli ultimi due sono trasporto, e sono quelli che si dimenticano.

## Cosa non ti arriva, e non è una dimenticanza

| | |
| --- | --- |
| `EarthShotFired` · `EarthShotMissed` · `EarthShotWasted` | cronaca del tiro colpo per colpo: riguarda la pagina, non te. Un bersaglio mancato non ti riguarda perché non c'è niente da fare — il cannone riprova da solo |
| Quanti colpi serva una stazza | è un conto di dominio, e il dominio è la Terra |
| Quanti colpi restano a un cannone | te lo dicono `FireOpened`, `FireCeased` e `CannonResupplied`, per quel cannone |
| Cosa stanno facendo gli altri processi | niente. Un processo vive per una nave sola |
