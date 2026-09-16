# ADR-0700: Structured logging

- Status: accepted
- Date: 2026-09-15
- Decision key: `structured-logging` = `yes`

## Context

Logs are the first thing consulted in an incident. Their format decides whether they can be filtered and aggregated or only searched as text.

## Decision

**Yes.** Emit machine-readable log records with stable field names through a single shared logger.

## Consequences

- Logs can be filtered, aggregated, and alerted on.
- A correlation identifier ties an operation together across components.
- Slightly less readable when tailed raw, and the field vocabulary needs upkeep.

## Alternatives considered

- **Plain text** — Easiest to read one line; impossible to query a million.

## Documents this decision produced

- Rules: [R-0230](../rules/0230-structured-logs-only.md)
- Guidelines: [GL-0090](../guidelines/0090-logging.md)

<!-- To change this decision, run `specframe revise structured-logging`: it records
     the old choice under History, refreshes the documents listed above, and
     reports the ones this decision no longer implies. -->
