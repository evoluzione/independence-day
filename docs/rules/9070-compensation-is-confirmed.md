# R-9070: Una compensazione va confermata, e finché non lo è il processo non chiude

- Status: enforced
- Source: [ADR-9070](../adr/9070-the-owner-decides.md), [ADR-9080](../adr/9080-failure-is-the-game.md)

## Rule

Un passo che impegna una risorsa di qualcun altro apre un debito. Il processo lo chiude mandando
l'azione compensativa **e aspettandone la conferma**.

Tre conseguenze, tutte obbligatorie:

1. La compensazione ha un evento di esito proprio, e quell'evento va ascoltato. Un'azione
   compensativa che non conferma è una speranza.
2. Il processo **non si chiude** quando l'obiettivo è raggiunto, ma quando non resta nessun debito
   aperto. Sono due condizioni separate e servono entrambe.
3. La compensazione va ritentata. Anche lei può perdersi, e va rimandata finché non conferma.

Lo stato della saga tiene l'elenco dei debiti aperti. Se un evento di apertura arriva quando
l'obiettivo è già risolto — l'ordine era per strada — quel debito va aperto e chiuso subito, non
ignorato.

## Why

È l'errore che non dà nessun errore. Chiudere il processo appena la nave cade sembra la cosa più
naturale del mondo, il codice legge bene, i test del caso felice passano, e a schermo non compare
niente di rosso. Intanto un cannone spara su un relitto per il resto della campagna — e quello che
costa non è la munizione, è il cannone che non c'è quando arriva la nave dopo.

Il difetto si manifesta a tre ondate di distanza dalla riga che lo ha causato, come una città che
cade per motivi che non si riescono a ricostruire. È esattamente il modo in cui questo tipo di bug si
presenta in produzione, ed è il motivo per cui il kata è costruito attorno a questo punto e non a un
altro.

Il caso di riferimento: nella prima stesura del processo di riferimento il `CompleteSaga` stava sul
ramo di `ShipDestroyed`. La campagna si perdeva al settimo livello con tre colpi su quattro sparati
contro navi già abbattute, e il diario era pieno di righe verdi.

## Enforcement

Revisione: se un processo manda un'azione compensativa, cerca l'handler del suo evento di conferma. Se
non c'è, è un difetto — non importa quanto sia improbabile la perdita.

Test: ogni processo con una compensazione ha due test. Uno prova che **non chiude** finché la conferma
non arriva; l'altro è la prova del contrario, cioè che una versione che non compensa **fallisce**. Il
secondo è quello che accorge quando il gioco smette di insegnare: vedi `CampaignTests`.
