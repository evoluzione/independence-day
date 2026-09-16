# R-0130: Recorded events are immutable

- Status: enforced
- Source: [ADR-0320](../adr/0320-event-sourcing.md)

## Rule

An event that has been appended is never edited or deleted. Corrections are new compensating events; shape changes are new event versions.

## Why

The event log is the system of record. Rewriting it invalidates every projection derived from it and destroys the audit trail that motivated event sourcing.

## Enforcement

Append-only permissions on the event store plus code review.
