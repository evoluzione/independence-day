# GL-0420: Components name what they do

- Status: active
- Source: [ADR-0130](../adr/0130-component-structure.md)

## Scope

Component and namespace names.

## Guideline

A component name states its role and responsibility without further reading. When the name leaves the question open, change the name — and check whether the namespace above it is wrong too.

## Rationale

The namespace path is the first thing a new colleague and an agent both read to decide where a change belongs. A name that needs explaining sends every one of those decisions somewhere else.

## Examples

Prefer:

```
customer.billing.history
ticket.assignment.routing
```

Avoid:

```
ticket.manager
customer.util.helper
```
