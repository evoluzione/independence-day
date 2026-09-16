# ADR-0310: Data ownership across services

- Status: accepted
- Date: 2026-09-15
- Decision key: `data-ownership` = `db-per-service`

## Context

Independent deployment is possible only when services do not share a schema. Whatever the intention, a shared database makes the boundary decorative.

## Decision

**Database per service.** Each service owns a private schema that no other service may read or write directly.

## Consequences

- Each service migrates and scales its storage independently.
- Data needed by several services is obtained through APIs or replicated by events, and is eventually consistent.
- Queries that would have been a join become an orchestration or a maintained projection.

## Alternatives considered

- **Shared database** — Keeps queries and transactions simple, and gives up the independence that motivated splitting.

## Documents this decision produced

- Rules: [R-0090](../rules/0090-no-cross-service-db.md), [R-0100](../rules/0100-service-owns-its-data.md)

<!-- To change this decision, run `specframe revise data-ownership`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
