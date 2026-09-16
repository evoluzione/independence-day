# ADR-0300: Primary persistence

- Status: accepted
- Date: 2026-09-15
- Decision key: `persistence` = `mixed`

## Context

The storage model determines which access patterns are natural and which require workarounds, and it is usually the hardest infrastructure choice to reverse once data has accumulated.

## Decision

**Mixed (polyglot).** Use more than one storage technology, choosing per capability, with one clearly authoritative store per piece of state.

## Consequences

- Each access pattern gets a store that suits it.
- More infrastructure to operate, and every duplication of state needs an owner and a synchronisation path.

## Alternatives considered

- **Relational** — Strongest guarantees and query flexibility; schema change is a managed process.
- **Document** — Natural fit for aggregate-shaped data; weaker at relationships and reporting.
- **Key-value** — Fastest and simplest at scale, only if every read is by key.
- **No persistence here** — Nothing to operate and nothing to migrate; the moment something must be remembered, this decision comes back.

## Documents this decision produced

- Guidelines: [GL-0410](../guidelines/0410-persistence-conventions.md)

<!-- To change this decision, run `specframe revise persistence`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
