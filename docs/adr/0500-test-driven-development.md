# ADR-0500: Test-driven development

- Status: accepted
- Date: 2026-09-15
- Decision key: `tdd` = `pragmatic`

## Context

When tests are written relative to the code determines what they are worth: written first they specify behaviour and shape the design, written after they tend to confirm the implementation, bugs included.

## Decision

**Pragmatic.** Write tests first for domain logic and for every bug fix; allow tests after the fact for exploratory or presentation code. Every change ships with tests.

## Consequences

- Keeps the design benefit where logic is dense, without forcing it on spikes and UI wiring.
- The line between the two modes is a judgement call and needs review attention.

## Alternatives considered

- **Strict TDD** — Highest design and regression value; the most demanding discipline to sustain.
- **Tests after implementation** — Zero adoption cost; the tests protect the code rather than the behaviour.

## Documents this decision produced

- Rules: [R-0070](../rules/0070-regression-test-for-every-fix.md)
- Guidelines: [GL-0290](../guidelines/0290-tdd-loop.md)

<!-- To change this decision, run `specframe revise tdd`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
