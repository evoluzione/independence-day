# ADR-0230: Dependency injection

- Status: accepted
- Date: 2026-09-15
- Decision key: `dependency-injection` = `container`

## Context

Every component needs collaborators. How they arrive determines whether coupling is visible in the signature or hidden in the body.

## Decision

**DI container.** Wire dependencies through a container that resolves the object graph from declared registrations.

## Consequences

- Wiring is centralised and lifetimes are managed in one place.
- The graph becomes implicit: resolution failures surface at runtime, and the container is another thing to learn.

## Alternatives considered

- **Manual constructor injection** — Most explicit and easiest to test; the wiring code grows by hand.
- **Direct construction** — Simplest until the first thing needs replacing in a test.

## Documents this decision produced

- Guidelines: [GL-0230](../guidelines/0230-dependency-injection.md)

<!-- To change this decision, run `specframe revise dependency-injection`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
