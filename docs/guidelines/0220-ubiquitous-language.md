# GL-0220: Ubiquitous language

- Status: active
- Source: [ADR-0210](../adr/0210-domain-driven-design.md)

## Scope

Domain code, tests, APIs, and conversation.

## Guideline

Use the domain expert's term in code, tests, and interfaces — one term per concept, one concept per term. When a term is contested, resolve it in docs/glossary/ and rename the code to match. Different bounded contexts may legitimately use the same word differently; say which context you mean.

## Rationale

Every translation between the business term and the code term is a place where a misunderstanding can hide indefinitely.
