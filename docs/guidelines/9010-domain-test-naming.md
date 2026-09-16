# GL-9010: Un file per scenario, il nome dice il caso

- Status: active
- Source: [ADR-9040](../adr/9040-battle-outcome-outside-the-aggregate.md)

## Scope

I test di dominio dei contesti e i test della saga.

## Guideline

Un test di dominio sta in un file suo, e il file si chiama come lo scenario:

```
Comando_QuandoCondizione_Esito.cs
```

La classe ha lo stesso nome del file e deriva dalla `CommandSpecification` del contesto: `Given()` sono
gli eventi già accaduti, `When()` il comando, `Expect()` gli eventi che devono uscire — nessuno, quando
la regola si rifiuta in silenzio.

I rami che decidono un esito si testano **entrambi**, sostituendo `IBattleOutcome` con
`FixedBattleOutcome`. Il caso nominale da solo non dimostra niente su una scala di escalation: è il
ramo negativo che la fa salire.

## Rationale

Il nome del file è il primo posto in cui si guarda quando un test rosso arriva da una pipeline, ed è
l'unico posto in cui può stare la condizione per intero. Un file con dieci `[Fact]` costringe a leggere
il corpo per sapere quale caso si è rotto.

## Examples

Prefer:

```
LaunchInterceptors_WhenNoInterceptorsLeft_ReportsTheLossWithoutLaunching.cs
FireNuke_WhenMotherShipIsStillShielded_DeflectsTheWarhead.cs
```

Avoid:

```
ShipInterceptionTests.cs
InterceptorTests.cs
```
