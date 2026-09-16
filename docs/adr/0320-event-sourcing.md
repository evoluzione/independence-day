# ADR-0320: Event sourcing

- Status: accepted
- Date: 2026-09-15
- Decision key: `event-sourcing` = `yes`

## Context

Persisting current state discards how it was reached. Event sourcing keeps the full sequence as the system of record and derives state from it, which is powerful and materially more expensive.

## Decision

**Yes, events are the system of record.** Persist state changes as an append-only sequence of domain events. Current state is derived by replaying them, and read models are projections that can be rebuilt at any time.

## Consequences

- Complete, auditable history: any past state can be reconstructed, and new read models can be backfilled from day one.
- Event schemas are permanent — every shape needs a versioning and upcasting strategy.
- Reads require a projection, and projections lag behind writes.
- Onboarding is slower: the model is unfamiliar to most developers.

## Alternatives considered

- **No, store current state** — Simplest and most queryable; past states are gone unless something explicitly recorded them.

## Documents this decision produced

- Rules: [R-0130](../rules/0130-events-are-immutable.md)
- Guidelines: [GL-0240](../guidelines/0240-event-design-and-versioning.md), [GL-0250](../guidelines/0250-projection-rebuilds.md)
- Runbooks: [RB-0040](../runbook/0040-rebuild-projections.md)

<!-- To change this decision, run `specframe revise event-sourcing`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
