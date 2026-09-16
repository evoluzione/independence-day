# GL-0070: Design patterns as shared vocabulary

- Status: active
- Source: [ADR-0220](../adr/0220-design-patterns.md)

## Scope

Design discussions and code review.

## Guideline

Reach for a pattern when it names a structure the code already needs — Strategy, Adapter, Facade, Repository, State, Observer. Name it in the code so the intent survives. Do not design pattern-first.

## Rationale

The value is the shared name, not the structure: it turns a paragraph of explanation into one word. Applied speculatively the same patterns add indirection with no reader to benefit from it.

## Examples

Avoid:

```
A God Object; a Singleton used as mutable global state; an AbstractFactoryFactory with one implementation.
```
