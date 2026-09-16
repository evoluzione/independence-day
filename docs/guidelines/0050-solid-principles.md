# GL-0050: SOLID principles applied

- Status: active
- Source: [ADR-0410](../adr/0410-solid-principles.md)

## Scope

Object-oriented and module-level design.

## Guideline

One module, one reason to change. Extend behaviour without modifying callers. Subtypes honour their supertype's contract. Interfaces are small and shaped by the client that uses them. Depend on abstractions, not concretions.

## Rationale

Applied as a review lens rather than a checklist, these five point at the specific coupling that makes a change expensive.
