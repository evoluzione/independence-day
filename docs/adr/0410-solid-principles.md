# ADR-0410: SOLID principles

- Status: accepted
- Date: 2026-09-15
- Decision key: `solid` = `yes`

## Context

SOLID names five recurring sources of expensive change. Adopting it explicitly gives review a vocabulary for coupling problems.

## Decision

**Yes.** Apply SOLID as a design and review lens, invoked when coupling is the actual problem.

## Consequences

- Gives reviewers precise language for why a change is expensive.
- Applied mechanically it produces interfaces with one implementation and needless indirection.

## Alternatives considered

- **No** — Less ceremony, weaker vocabulary for design review.

## Documents this decision produced

- Guidelines: [GL-0050](../guidelines/0050-solid-principles.md)

<!-- To change this decision, run `specframe revise solid`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
