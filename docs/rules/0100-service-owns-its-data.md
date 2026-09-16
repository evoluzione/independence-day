# R-0100: A service owns its data

- Status: enforced
- Source: [ADR-0100](../adr/0100-architecture-style.md), [ADR-0310](../adr/0310-data-ownership.md)

## Rule

Exactly one service writes any given piece of state. Every other component obtains it through an API call or a published event.

## Why

Single ownership is what makes a change to storage a local decision. Two writers means no one can reason about invariants.

## Enforcement

Code review; ownership is recorded in the service catalog.
