# R-9070: Una compensazione va confermata, e finché non lo è il processo non chiude

- Status: enforced
- Source: [ADR-9070](../adr/9070-the-owner-decides.md), [ADR-9080](../adr/9080-failure-is-the-game.md)

## Rule

Un passo che impegna una risorsa di qualcun altro apre un debito. Il processo lo chiude mandando
l'azione compensativa **e aspettandone la conferma**.

Due conseguenze, entrambe obbligatorie:

1. La compensazione ha un evento di esito proprio, e quell'evento va ascoltato. Un'azione
   compensativa che non conferma è una speranza.
2. Il processo **non si chiude** quando l'obiettivo è raggiunto, ma quando non resta nessun debito
   aperto. Sono due condizioni separate e servono entrambe.

Lo stato della saga tiene l'elenco dei debiti aperti. Se un evento di apertura arriva quando
l'obiettivo è già risolto — l'ordine era per strada mentre la nave cadeva — quel debito va aperto e
chiuso subito, non ignorato.

> **Amendment 2026-09-18.** Un terzo punto imponeva di ripetere la compensazione perché anche lei
> poteva perdersi sul collegamento. È caduto con l'ordine perso stesso — vedi
> [ADR-9080](../adr/9080-failure-is-the-game.md). Un cessate il fuoco che arriva a destinazione non
> ha più bisogno di essere insistito.

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

Test: `ShipInterceptionSagaTests` prova che la compensazione parte quando la nave è risolta.
`CampaignTests` è la prova del contrario su scala: gioca la stessa ondata cinque volte, aggiungendo un
comportamento alla volta, e pretende che ogni aggiunta — compensazione compresa — abbatta più navi
della precedente. Se restituire i cannoni diventasse decorativo, quel gradino smetterebbe di
guadagnare navi, e il test lo direbbe.
