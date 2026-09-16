# GL-9000: Un contesto, cinque progetti

- Status: active
- Source: [ADR-9000](../adr/9000-service-boundaries.md)

## Scope

I contesti di dominio sotto `src/` — oggi `Space` e `Earth`.

## Guideline

Ogni contesto si divide sempre allo stesso modo, e i progetti si chiamano come la loro parte:

| Progetto | Contiene | Non contiene |
| --- | --- | --- |
| `.Messages` | Comandi, eventi di dominio, `DomainIds`. Il vocabolario. | Logica |
| `.Domain` | Aggregati, servizi di dominio, command handler | Accesso a Mongo o al bus |
| `.ReadModel` | Documenti, proiezioni, traduzioni verso il bus | Regole di dominio |
| `.Facade` | Registrazione DI, endpoint, hosted service, handler di integrazione | Regole di dominio |
| `.Host` | `Program.cs`, `Dockerfile`, `appsettings.json`, la pagina statica | Logica |
| `.Domain.Tests` | Un file per scenario | — |

Quello che attraversa il confine del contesto non sta qui ma in `Shared/Contracts`, e gli adattatori
condivisi — bus, event store, Mongo — in `Shared/Infrastructure`.

L'host sta sotto la cartella del proprio contesto, non in una cartella `Hosts/` a parte: il deployable
e' parte del contesto quanto i suoi aggregati, e tenerlo altrove costringe a saltare fra due rami
dell'albero per seguire una modifica. Non contiene logica: chiama `AddSpace(configuration)` o
`AddEarth(configuration)` e mappa gli endpoint.

## Rationale

La divisione dice dove cercare prima ancora di aprire un file, e regge la regola che conta: `Domain`
non referenzia `Infrastructure` se non per il repository, quindi non c'è modo di scrivere una regola di
business che dipenda da Mongo senza accorgersene.

Il repository di riferimento aggiunge `.SharedKernel` e un progetto `Mediator` per contesto. Qui non
servono: non c'è una forma condivisa fra scrittura e lettura abbastanza grande da giustificare un
progetto, e il mediator esiste per comporre due contesti, cosa che qui non succede mai.

## Examples

Prefer:

```
src/Earth/Evoluzione.IndependenceDay.Earth.Messages/Events/CityShieldsRaised.cs
src/Earth/Evoluzione.IndependenceDay.Earth.Domain/Entities/EarthDefense.cs
src/Earth/Evoluzione.IndependenceDay.Earth.Domain/Services/IBattleOutcome.cs
src/Earth/Evoluzione.IndependenceDay.Earth.Host/Program.cs
src/Shared/Evoluzione.IndependenceDay.Contracts/Events/ShieldsRaised.cs
```

Avoid:

```
src/Earth/Evoluzione.IndependenceDay.Earth.Domain/MongoDB/CityRepository.cs
src/Earth/Evoluzione.IndependenceDay.Earth.Facade/Rules/EscalationRules.cs
```
