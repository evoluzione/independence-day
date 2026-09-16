# ADR-0210: Domain-driven design

- Status: accepted
- Date: 2026-09-15
- Decision key: `ddd` = `full`

## Context

Domain complexity has to live somewhere. DDD puts it in an explicit model with an agreed vocabulary; without it, that complexity distributes itself across the code as implicit rules.

## Decision

**Full DDD.** Adopt DDD strategically and tactically: identify bounded contexts, maintain a ubiquitous language, and model with aggregates, entities, and value objects.

## Consequences

- Code, tests, and conversation share one vocabulary, which removes a whole class of misunderstanding.
- Aggregate boundaries give a principled answer to transaction scope and, later, to service boundaries.
- Requires sustained access to domain experts; without it the model becomes invented rather than discovered.

## Alternatives considered

- **Tactical patterns only** — Most of the modelling value for a fraction of the process, at the cost of vocabulary drift between areas.
- **No DDD** — Appropriate when the domain is thin; costly when it turns out not to be.

## Documents this decision produced

- Guidelines: [GL-0210](../guidelines/0210-ddd-tactical-patterns.md), [GL-0220](../guidelines/0220-ubiquitous-language.md)

<!-- To change this decision, run `specframe revise ddd`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
