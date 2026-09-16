# ADR-0340: Consistency across boundaries

- Status: accepted
- Date: 2026-09-15
- Decision key: `distributed-transactions` = `saga-orchestration`

## Context

A change spanning two owners cannot be atomic. What remains is a choice about where the coordination lives and how partial failure is repaired.

## Decision

**Orchestrated saga.** Model cross-boundary workflows as sagas driven by an explicit coordinator that invokes each step and its compensation.

## Consequences

- The workflow exists in one readable place, and its state is inspectable when it stalls.
- The coordinator is a dependency of the whole process and needs its own availability story.

## Alternatives considered

- **Transactional outbox** — Solves the lost-message problem with minimal machinery; does not coordinate multi-step workflows.
- **Choreographed saga** — Loosest coupling; the process becomes emergent and difficult to reason about end to end.
- **Avoid cross-boundary transactions** — By far the simplest, and only viable while boundaries can absorb the constraint.

## Documents this decision produced

- Rules: [R-0300](../rules/0300-outbox-for-cross-service-writes.md)
- Guidelines: [GL-0270](../guidelines/0270-saga-design.md)

<!-- To change this decision, run `specframe revise distributed-transactions`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
