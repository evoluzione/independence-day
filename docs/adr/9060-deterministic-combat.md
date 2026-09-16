# ADR-9060: Combattimento deterministico a forze contabili

- Status: accepted
- Date: 2026-09-15
- Supersedes: [ADR-9030](9030-defense-escalation-ladder.md), [ADR-9040](9040-battle-outcome-outside-the-aggregate.md)

## Context

Questo repository viene consegnato a delle squadre: ognuna scrive la propria saga e vince chi resiste
piu' ondate. Perche' il confronto abbia senso, due cose devono essere vere.

La prima: **si deve vedere tutto**. Le regole precedenti — scudi che reggono o no, un virus che
attecchisce o viene respinto — avevano esiti che non comparivano da nessuna parte se non come una
riga di diario. Chi guardava non poteva dire perche' una difesa fosse andata bene, e chi progettava
una saga non aveva numeri su cui ragionare.

La seconda: **lo stesso piano deve dare lo stesso risultato**. Con un esito probabilistico, due
squadre con la stessa saga arrivano a livelli diversi, e la classifica misura la fortuna quanto il
progetto. [ADR-9040](9040-battle-outcome-outside-the-aggregate.md) aveva reso il tiro ripetibile
seminandolo sugli identificativi, che risolve la reidratazione ma non il confronto: due flotte
identiche restano diverse perche' hanno id diversi.

## Decision

**Una riserva unica di quattro forze contabili — soldati, carri armati, aerei, atomiche — che un
ordine piazza su una citta', e una regola sola: le unita' piazzate affrontano da sole le flotte che
arrivano li', si consumano, e ognuna abbatte un numero fisso di navi.**

| Forza | Navi per unita' | In riserva |
| --- | --- | --- |
| 🎖️ Soldati | 1 | 750 |
| 🚜 Carri armati | 3 | 200 |
| ✈️ Aerei | 6 | 100 |
| ☢️ Atomiche | 40 | 15 |

La riserva e' unica per tutte le citta', quindi la difesa e' **un aggregato solo**: "non puoi piazzare
piu' di quello che hai" e' un invariante che le attraversa tutte, e un aggregato e' il confine di un
invariante.

Niente dadi, niente scudi, niente virus: nessun effetto che non sia un numero sullo schermo. Una
flotta non fermata entro il tempo di avvicinamento tocca terra e toglie integrita' alla citta'.

La citta' spende sempre prima le forze piu' economiche: un'atomica lasciata li' non viene bruciata
contro tre navi. La saga sceglie <b>cosa piazzare e dove</b>, non come viene speso.

Una citta' senza forze cade alla prima flotta: quindici punti di danno per nave, su cento di
integrita'. Non difendersi non e' un'opzione con un costo, e' la sconfitta.

## Consequences

- Il gioco diventa un problema di **allocazione e anticipo**: quante forze impegnare, su quale citta',
  e soprattutto quando — chi piazza prima che la flotta arrivi la trova gia' pronta.
- E' interamente leggibile: a schermo ci sono la riserva, le forze su ogni citta' e le flotte in volo.
- Due squadre con la stessa saga arrivano allo stesso livello. La classifica misura il progetto.
- `IBattleOutcome` sparisce, e con lui il servizio di dominio che [ADR-9040](9040-battle-outcome-outside-the-aggregate.md)
  introduceva. L'aggregato torna a essere una funzione pura dei suoi eventi senza bisogno di aiuto.
- La scala di escalation di [ADR-9030](9030-defense-escalation-ladder.md) non e' piu' una regola del
  dominio: quale forza spendere e dove e' la **scelta della saga**, ed e' quella su cui si compete.
- Un aggregato solo per la Terra serializza le scritture della difesa. A questi volumi non si sente,
  e il prezzo e' pagato per avere l'invariante della riserva dove deve stare.
- Si perde l'imprevedibilita': una partita giocata due volte allo stesso modo finisce allo stesso
  modo. E' un prezzo accettato — la varieta' la porta la curva di difficolta', non il caso.

## Alternatives considered

- **Tenere il tiro seminato** — Ripetibile per lo stesso comando ma non fra flotte diverse: due
  squadre con la stessa saga finirebbero a livelli diversi, e il confronto perderebbe senso.
- **Percentuali di perdita per tipo di unita'** — Piu' realistico, ma aggiunge un numero da spiegare
  per ogni forza e nulla che si veda a schermo. "Le unita' impegnate si consumano" e' una regola
  sola, e si capisce al primo sguardo.

## Documents this decision produced

- Rules: [R-9010](../rules/9010-aggregates-do-not-throw.md)
- Guidelines: —
