# R-0330: Forbidden dependencies between components stay forbidden

- Status: enforced
- Source: [ADR-0130](../adr/0130-component-structure.md)

## Rule

A component reaches another only where the design allows it, and never into its internals. Each forbidden pair is written down and checked separately.

## Why

Coupling is added one import at a time, always for a good local reason. Naming the edges that must not exist is what keeps the dependency graph a design rather than a record of expedience.

## Enforcement

One CI check per forbidden pair, plus code review.
