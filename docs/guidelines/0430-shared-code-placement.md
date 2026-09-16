# GL-0430: Shared code is a component, not a parent node

- Status: active
- Source: [ADR-0140](../adr/0140-shared-code.md)

## Scope

Interfaces, abstract classes, and utilities used by more than one component.

## Guideline

Shared code goes into its own leaf component, never into the parent namespace of the components that use it. Reserve one suffix for it and use that suffix for nothing else. Keep shared domain logic — notification, formatting, validation — apart from shared infrastructure — logging, metrics, security: the first is business logic common to some components, the second is operational and common to all of them.

## Rationale

A suffix used for nothing else turns sharing from a feeling into a measurement: what share of the codebase is shared, and across how many components. Approaching 40% is a cohesion problem now, not at some future extraction. And the two kinds of sharing have different futures — infrastructure travels with every deployment unit, shared domain logic has to be assigned to one.

## Examples

Prefer:

```
customer.billing.sharedcode
platform.infrastructure.logging
```

Avoid:

```
customer.billing/  (interfaces and abstract classes, plus three child packages)
```
