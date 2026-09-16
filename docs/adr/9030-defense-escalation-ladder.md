# ADR-9030: La scala di escalation delle difese

- Status: superseded
- Superseded by: [ADR-9060](9060-deterministic-combat.md)
- Date: 2026-09-15

## Superata da ADR-9060

L'ordine fisso non e' piu' una regola del dominio: le quattro forze si possono impegnare in
qualunque ordine, e sceglierlo e' il lavoro della saga. Quello che qui era la decisione centrale e'
diventato lo spazio di gioco delle squadre.

## Context

La Terra ha quattro contromisure: scudi, intercettori, virus sulla nave madre, atomica. Il modo in cui
vengono usate è la regola di dominio centrale di questo repository, e ci sono più modi credibili di
metterle in fila.

Si potrebbero lanciare **tutte insieme** — massima probabilità di fermare la flotta, ma ogni battaglia
diventa identica alle altre e l'esito non racconta più niente. Si potrebbe **scegliere in base alla
minaccia** — una flotta piccola merita solo gli intercettori, una grande direttamente l'atomica — e
sarebbe la scelta più realistica, ma introduce una funzione di valutazione che nessun evento registra,
quindi due esecuzioni della stessa battaglia potrebbero divergere.

## Decision

**Ordine fisso, un gradino alla volta:**

```
scudi → intercettori → virus sulla nave madre → atomica → resa
```

Si sale solo quando il gradino corrente ha fallito, e ogni gradino ha il proprio evento di esito
negativo. L'atomica non scalfisce una nave madre con lo scudo attivo: è il virus ad aprirlo, ed è
l'aggregato a rifiutarsi con `NukeDeflected` se qualcuno prova a saltare il passaggio.

## Consequences

- Lo stato della saga è un enum con cinque valori, e la posizione nel giro si legge senza interpretare.
- Un evento che appartiene a un gradino già superato è un ritardatario, non un ordine di tornare
  indietro: la saga lo scarta guardando il gradino.
- La sequenza è prevedibile, quindi verificabile: i test dei rami vittoria e sconfitta sono entrambi
  scrivibili senza toccare il caso.
- L'ordine è codificato in due posti — nella saga che manda i comandi e nell'aggregato che rifiuta
  l'atomica prematura. Sono d'accordo per costruzione ma non per meccanismo: un cambio di scala va
  fatto in entrambi.

## Alternatives considered

- **Tutte le contromisure in parallelo** — Massima efficacia, nessuna storia da raccontare: ogni flotta
  finisce allo stesso modo e la saga si riduce a un fan-out.
- **Scelta adattiva in base alla dimensione della flotta** — Più realistica, ma la decisione dipende da
  una valutazione che nessun evento registra: alla rilettura del log l'esito potrebbe non tornare.

## Documents this decision produced

- Rules: [R-9010](../rules/9010-aggregates-do-not-throw.md)
- Guidelines: —
