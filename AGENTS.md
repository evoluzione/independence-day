# Per chi lavora qui dentro, umano o meno

Questo repository è un kata: il codice è **materiale didattico**, non solo software che funziona. Chi
lo legge sta imparando qualcosa, quindi le solite priorità si spostano.

## Prima di toccare

Leggi [ARCHITETTURA.md](ARCHITETTURA.md) e [REGOLE.md](REGOLE.md). Le decisioni stanno in
[docs/adr/](docs/adr/), quello che in revisione è un difetto in [docs/rules/](docs/rules/).

Il confine che conta: **sotto `src/Sagas/` c'è l'esercizio**, tutto il resto è il campo di gioco.
Cambiare un numero in `Contracts/World/` o nella curva di difficoltà cambia il gioco per tutti, e
invalida le tabelle in `REGOLE.md`.

## Come si scrive

**I commenti spiegano il perché, mai il cosa.** Il cosa si legge dal codice. Un commento che
riassume la riga sotto è rumore; uno che dice perché non è stata fatta nel modo ovvio vale il suo
spazio. Vedi [G-0030](docs/guidelines/0030-comments-explain-why.md).

**La prosa è in italiano**, nei commenti come nei documenti. Gli identificatori sono in inglese.
Nella documentazione XML non si usano lettere accentate: `citta'`, non `città`.

**Un aggregato non solleva.** Un ordine impossibile è un evento, non un'eccezione
([R-9010](docs/rules/9010-aggregates-do-not-throw.md)).

**Una proiezione non rilancia.** Rilanciare blocca la sottoscrizione per tutti gli eventi successivi,
e la partita si ferma senza che a schermo si veda niente
([R-9020](docs/rules/9020-projections-never-rethrow.md)).

**Il progetto delle saghe non ospita dominio.** Nessuna costante di gioco, nessun conto: quelli
stanno nell'aggregato che possiede la risorsa. Una saga è due file, lo stato e la saga
([R-9060](docs/rules/9060-saga-does-not-decide.md)).

## Prima di dire che è fatto

```bash
docker compose --profile test run --rm tests
```

Se hai toccato un numero del gioco, guarda cosa dice la simulazione della campagna: dev'essere ancora
vera sia la vittoria sia il suo contrario, cioè che **senza compensazione si perde**. Se il secondo
test diventa verde, il gioco non insegna più niente.

Se hai toccato il giro end-to-end, giocaci davvero:

```bash
docker compose up --build   # poi http://localhost:8080
```

## Se aggiungi un evento

Quattro passi, e saltarne uno non dà errore: l'evento semplicemente non arriva mai. La ricetta sta in
[EVENTI.md](EVENTI.md), sezione *Come si aggancia un evento*.

## Se cambi le regole

`REGOLE.md` ed `EVENTI.md` sono la specifica che i partecipanti leggono, e vanno tenuti allineati al
codice nello stesso commit. Una tabella sbagliata lì costa più di un test rotto: un test rotto si
vede.
