# GL-0010: Naming conventions

- Status: active
- Source: [ADR-0400](../adr/0400-clean-code.md)

## Scope

All code.

## Guideline

Names are explicit, pronounceable, and searchable. Functions read as verb + object; classes and modules are nouns; booleans use an `is` / `has` / `can` / `should` prefix. Abbreviate only where the short form is the domain term.

## Rationale

Naming is the cheapest documentation available and the only one that cannot go stale without the compiler noticing.

## Examples

Prefer:

```
function findOverdueInvoices(customerId)
const hasActiveSubscription = ...
```

Avoid:

```
function proc(x)
const flag = ...
```
