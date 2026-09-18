# ADR-9050: Tutto gira in container, compose e' l'unico comando d'avvio

- Status: accepted
- Date: 2026-09-15

## Context

Il repository di riferimento mette in compose la sola infrastruttura — basi dati, broker, stub — e
lascia che l'applicazione giri sull'host con `dotnet run`. È la scelta giusta per chi sviluppa tutto il
giorno su quel codice: il ciclo di modifica è immediato e il debugger è attaccato.

Qui l'obiettivo è un altro. Questo repository deve mostrare un'architettura a chi lo apre per la prima
volta, e l'unica prova che l'architettura funziona è vederla girare. Ogni passaggio fra il clone e la
prima flotta è un punto in cui qualcuno si ferma: installare l'SDK, scoprire di avere la versione
sbagliata, capire quale progetto avviare per primo.

## Decision

**Anche i tre servizi .NET sono servizi di compose, costruiti da un Dockerfile multi-stage
sull'immagine SDK.** `docker compose up --build` è l'unico comando necessario, e sulla macchina servono
soltanto Docker e il repository. I test girano allo stesso modo, sotto il profilo `test`, nell'immagine
SDK con il sorgente montato.

**La saga fa eccezione: è il codice che si scrive, quindi gira direttamente nell'immagine SDK sotto
`dotnet watch`, con `src/` montato.** Un salvataggio ricompila e riavvia il processo da solo: senza
questo, ogni singola modifica al kata costerebbe un `--build` dell'intera solution. Il watcher usa il
polling, perché su bind mount Docker gli eventi inotify del filesystem non arrivano al container.

Il watcher gira con `--no-hot-reload`: la patch a caldo applicherebbe il codice nuovo a un container
DI già costruito all'avvio, e la modifica risulterebbe compilata ma non ascoltata — il modo peggiore
di sbagliare, perché sembra che il codice non faccia niente. Un riavvio pieno costa qualche secondo e
non mente.

Le porte pubblicate stanno fuori dagli standard (2114/2115, 5673/15673, 27018) perché è normale avere
già un Mongo o un Rabbit in ascolto, e un conflitto di porta si presenta come un servizio che non parte.

## Consequences

- Dal clone alla prima flotta non c'è niente da installare e niente da configurare.
- Sulla saga il ciclo è quello di `dotnet watch`: si salva e basta. Su Spazio e Terra ogni modifica
  richiede ancora un `--build`, ed è accettabile perché non sono il codice dell'esercizio. Chi ci lavora
  davvero può avviare l'host dall'IDE contro la stessa infrastruttura, perché gli `appsettings.json`
  puntano a `localhost` e sono le variabili d'ambiente di compose a riscriverli.
- La saga non ha più un Dockerfile: gira nell'immagine SDK, non in quella runtime. È un container di
  sviluppo, non un artefatto di produzione — che qui non esiste.
- `src/bin` e `src/obj` sono condivisi fra host e container, come già per il profilo `test`: chi compila
  anche sulla macchina paga un restore in più a ogni passaggio di lato.
- Le immagini si costruiscono con il contesto su `src/`, quindi il restore vede tutta la solution: una
  build è più lenta di quella di un singolo progetto.
- Non c'è nessun passaggio manuale da documentare, quindi nessun passaggio che possa restare indietro
  rispetto al codice.

## Alternatives considered

- **Solo l'infrastruttura in compose**, come nel repository di riferimento. Ciclo di sviluppo molto più
  veloce, ma richiede .NET sulla macchina e la conoscenza di quali host avviare: esattamente quello che
  questo repository non vuole chiedere.
- **Immagini pre-costruite su un registry** — Avvio immediato, ma il codice nel repository e l'immagine
  che gira possono divergere senza che nulla lo segnali.

## Documents this decision produced

- Rules: —
- Guidelines: —
