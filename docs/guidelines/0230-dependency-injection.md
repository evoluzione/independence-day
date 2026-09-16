# GL-0230: Dependency injection

- Status: active
- Source: [ADR-0230](../adr/0230-dependency-injection.md)

## Scope

Object construction and wiring.

## Guideline

Register dependencies with the container and resolve only at the composition root. Construct dependencies at the edge and pass them inward. Depend on the narrow interface a component actually needs, and never reach for a global or a service locator inside domain code.

## Rationale

Explicit dependencies make a component's real coupling visible in its signature, and make substitution in tests a matter of passing a different value.
